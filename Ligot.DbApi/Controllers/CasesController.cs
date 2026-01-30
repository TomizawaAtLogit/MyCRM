using Ligot.DbApi.Models;
using Ligot.DbApi.Repositories;
using Ligot.DbApi.DTOs;
using Ligot.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ligot.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Policy = "SupportOnly")] // Commented for local development
    public class CasesController : AuditableControllerBase
    {
        private readonly ICaseRepository _repo;
        private readonly ICaseActivityRepository _activityRepo;
        private readonly ISlaConfigurationRepository _slaRepo;
        private readonly IAuditService _auditService;
        
        public CasesController(
            ICaseRepository repo, 
            ICaseActivityRepository activityRepo,
            ISlaConfigurationRepository slaRepo,
            IUserRepository userRepo, 
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
            _slaRepo = slaRepo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<CaseDto>> Get(
            [FromQuery] int? customerId, 
            [FromQuery] int? systemId,
            [FromQuery] int? systemComponentId,
            [FromQuery] int? customerSiteId,
            [FromQuery] int? customerOrderId,
            [FromQuery] CaseStatus? status,
            [FromQuery] CasePriority? priority,
            [FromQuery] int? assignedToUserId,
            [FromQuery] bool includeOverdue = false,
            [FromQuery] bool includeSlaBreached = false)
        {
            IEnumerable<Case> cases;
            
            // Get current user and their allowed customer IDs
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            if (!userId.HasValue)
            {
                // If user is not found, return empty list
                return Enumerable.Empty<CaseDto>();
            }
            
            var allowedCustomerIds = await _userRepo.GetAllowedCustomerIdsAsync(userId.Value);
            
            if (customerId.HasValue)
            {
                // Check if the requested customer is allowed
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0 && !allowedCustomerIds.Contains(customerId.Value))
                {
                    // User doesn't have access to this customer
                    return Enumerable.Empty<CaseDto>();
                }
                cases = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else if (assignedToUserId.HasValue)
            {
                cases = await _repo.GetByAssignedUserIdAsync(assignedToUserId.Value);
                // Apply coverage filter after getting by assigned user
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    cases = cases.Where(c => allowedCustomerIds.Contains(c.CustomerId));
                }
            }
            else if (status.HasValue)
            {
                cases = await _repo.GetByStatusAsync(status.Value);
                // Apply coverage filter after getting by status
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    cases = cases.Where(c => allowedCustomerIds.Contains(c.CustomerId));
                }
            }
            else
            {
                cases = await _repo.GetAllAsync(allowedCustomerIds);
            }

            // Apply additional filters
            if (systemId.HasValue)
                cases = cases.Where(c => c.SystemId == systemId.Value);
            
            if (systemComponentId.HasValue)
                cases = cases.Where(c => c.SystemComponentId == systemComponentId.Value);
            
            if (customerSiteId.HasValue)
                cases = cases.Where(c => c.CustomerSiteId == customerSiteId.Value);
            
            if (customerOrderId.HasValue)
                cases = cases.Where(c => c.CustomerOrderId == customerOrderId.Value);
            
            if (priority.HasValue)
                cases = cases.Where(c => c.Priority == priority.Value);

            if (includeOverdue)
            {
                var now = DateTime.UtcNow;
                cases = cases.Where(c => c.DueDate.HasValue && c.DueDate.Value < now && 
                                        (c.Status == CaseStatus.Open || c.Status == CaseStatus.InProgress));
            }

            var casesList = cases.ToList();
            var dtos = new List<CaseDto>();

            foreach (var c in casesList)
            {
                var slaConfig = await _slaRepo.GetByPriorityAsync(c.Priority);
                var dto = await BuildCaseDtoAsync(c, slaConfig);
                
                if (includeSlaBreached && !dto.IsResponseSlaBreached && !dto.IsResolutionSlaBreached)
                    continue;
                
                dtos.Add(dto);
            }

            return dtos;
        }

        [HttpGet("overdue")]
        public async Task<IEnumerable<CaseDto>> GetOverdue()
        {
            var cases = await _repo.GetOverdueCasesAsync();
            var dtos = new List<CaseDto>();
            
            foreach (var c in cases)
            {
                var slaConfig = await _slaRepo.GetByPriorityAsync(c.Priority);
                dtos.Add(await BuildCaseDtoAsync(c, slaConfig));
            }
            
            return dtos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaseDto>> Get(int id)
        {
            var c = await _repo.GetAsync(id);
            if (c == null) return NotFound();
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "Case", id, c);
            
            var slaConfig = await _slaRepo.GetByPriorityAsync(c.Priority);
            return await BuildCaseDtoAsync(c, slaConfig);
        }

        [HttpPost]
        public async Task<ActionResult<CaseDto>> Post(CreateCaseDto dto)
        {
            var caseEntity = new Case
            {
                Title = dto.Title,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                SystemId = dto.SystemId,
                SystemComponentId = dto.SystemComponentId,
                CustomerSiteId = dto.CustomerSiteId,
                CustomerOrderId = dto.CustomerOrderId,
                Status = dto.Status,
                Priority = dto.Priority,
                IssueType = dto.IssueType,
                AssignedToUserId = dto.AssignedToUserId,
                ResolutionNotes = dto.ResolutionNotes,
                DueDate = dto.DueDate
            };
            
            var created = await _repo.AddAsync(caseEntity);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "Case", created.Id, created);
            
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            var slaConfig = await _slaRepo.GetByPriorityAsync(result.Priority);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, await BuildCaseDtoAsync(result, slaConfig));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateCaseDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            var oldAssignedToUserId = existing.AssignedToUserId;
            var oldStatus = existing.Status;
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.CustomerId = dto.CustomerId;
            existing.SystemId = dto.SystemId;
            existing.SystemComponentId = dto.SystemComponentId;
            existing.CustomerSiteId = dto.CustomerSiteId;
            existing.CustomerOrderId = dto.CustomerOrderId;
            existing.Status = dto.Status;
            existing.Priority = dto.Priority;
            existing.IssueType = dto.IssueType;
            existing.AssignedToUserId = dto.AssignedToUserId;
            existing.ResolutionNotes = dto.ResolutionNotes;
            existing.DueDate = dto.DueDate;
            existing.UpdatedAt = DateTime.UtcNow;
            
            // Auto-set FirstResponseAt if status changes from Open
            if (existing.FirstResponseAt == null && oldStatus == CaseStatus.Open && dto.Status != CaseStatus.Open)
            {
                existing.FirstResponseAt = DateTime.UtcNow;
            }
            
            // Auto-set ResolvedAt and ClosedAt
            if ((dto.Status == CaseStatus.Resolved || dto.Status == CaseStatus.Closed) && existing.ResolvedAt == null)
            {
                existing.ResolvedAt = DateTime.UtcNow;
            }
            
            if (dto.Status == CaseStatus.Closed && existing.ClosedAt == null)
            {
                existing.ClosedAt = DateTime.UtcNow;
            }
            
            await _repo.UpdateAsync(existing);
            
            // Auto-create activity entry for assignment changes
            if (oldAssignedToUserId != dto.AssignedToUserId)
            {
                var activity = new CaseActivity
                {
                    CaseId = id,
                    ActivityDate = DateTime.UtcNow,
                    Summary = "Assignment changed",
                    ActivityType = "Assignment",
                    PerformedBy = username,
                    PreviousAssignedToUserId = oldAssignedToUserId,
                    NewAssignedToUserId = dto.AssignedToUserId
                };
                await _activityRepo.AddAsync(activity);
            }
            
            await _auditService.LogActionAsync(username, userId, "Update", "Case", id, existing);
            
            // Check for open related cases when closing
            var hasWarning = false;
            string? warningMessage = null;
            if (dto.Status == CaseStatus.Closed)
            {
                var openRelatedCount = await _repo.GetOpenRelatedCasesCountAsync(id);
                if (openRelatedCount > 0)
                {
                    hasWarning = true;
                    warningMessage = $"Warning: This case has {openRelatedCount} related open cases";
                }
            }
            
            if (hasWarning)
            {
                return Ok(new { warning = true, message = warningMessage });
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var caseEntity = await _repo.GetAsync(id);
            await _repo.DeleteAsync(id);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "Case", id, caseEntity);
            
            return NoContent();
        }

        [HttpPost("bulk-update")]
        public async Task<ActionResult<BulkUpdateCasesResponse>> BulkUpdate(BulkUpdateCasesDto dto)
        {
            var (username, userId) = await GetCurrentUserInfoAsync();
            var updatedIds = new List<int>();
            
            // Get all cases before update for audit
            var casesToUpdate = new List<Case>();
            foreach (var caseId in dto.CaseIds)
            {
                var c = await _repo.GetAsync(caseId);
                if (c != null) casesToUpdate.Add(c);
            }
            
            // Perform bulk updates
            if (dto.AssignedToUserId.HasValue)
            {
                var assignedIds = await _repo.BulkUpdateAssignmentAsync(dto.CaseIds, dto.AssignedToUserId.Value);
                updatedIds.AddRange(assignedIds);
                
                // Create individual audit entries
                foreach (var c in casesToUpdate)
                {
                    c.AssignedToUserId = dto.AssignedToUserId.Value;
                    c.UpdatedAt = DateTime.UtcNow;
                    await _auditService.LogActionAsync(username, userId, "BulkUpdate", "Case", c.Id, c);
                }
            }
            
            if (dto.Status.HasValue)
            {
                var statusIds = await _repo.BulkUpdateStatusAsync(dto.CaseIds, dto.Status.Value);
                updatedIds.AddRange(statusIds);
                
                // Create individual audit entries
                foreach (var c in casesToUpdate)
                {
                    c.Status = dto.Status.Value;
                    c.UpdatedAt = DateTime.UtcNow;
                    await _auditService.LogActionAsync(username, userId, "BulkUpdate", "Case", c.Id, c);
                }
            }
            
            var uniqueIds = updatedIds.Distinct().ToArray();
            
            return new BulkUpdateCasesResponse(
                uniqueIds.Length,
                uniqueIds,
                false,
                null
            );
        }

        private async Task<CaseDto> BuildCaseDtoAsync(Case c, SlaThreshold? slaConfig)
        {
            var responseTimeSlaMinutes = slaConfig?.ResponseTimeHours * 60;
            var resolutionTimeSlaMinutes = slaConfig?.ResolutionTimeHours * 60;
            
            var isResponseSlaBreached = false;
            var isResolutionSlaBreached = false;
            DateTime? slaDeadline = null;
            
            if (c.Status == CaseStatus.Open || c.Status == CaseStatus.InProgress)
            {
                if (responseTimeSlaMinutes.HasValue)
                {
                    var responseDeadline = c.CreatedAt.AddMinutes(responseTimeSlaMinutes.Value);
                    isResponseSlaBreached = !c.FirstResponseAt.HasValue && DateTime.UtcNow > responseDeadline;
                }
                
                if (resolutionTimeSlaMinutes.HasValue)
                {
                    var resolutionDeadline = c.CreatedAt.AddMinutes(resolutionTimeSlaMinutes.Value);
                    isResolutionSlaBreached = !c.ResolvedAt.HasValue && DateTime.UtcNow > resolutionDeadline;
                    slaDeadline = resolutionDeadline;
                }
            }
            
            var openRelatedCasesCount = await _repo.GetOpenRelatedCasesCountAsync(c.Id);
            
            return new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.CustomerId,
                c.Customer?.Name,
                c.SystemId,
                c.System?.SystemName,
                c.SystemComponentId,
                c.SystemComponent?.ComponentType,
                c.CustomerSiteId,
                c.CustomerSite?.SiteName,
                c.CustomerOrderId,
                c.CustomerOrder?.OrderNumber,
                c.Status,
                c.Priority,
                c.IssueType,
                c.AssignedToUserId,
                c.AssignedToUser?.DisplayName,
                c.ResolutionNotes,
                c.DueDate,
                c.FirstResponseAt,
                c.ResolvedAt,
                c.ClosedAt,
                c.CreatedAt,
                c.UpdatedAt,
                responseTimeSlaMinutes,
                resolutionTimeSlaMinutes,
                isResponseSlaBreached,
                isResolutionSlaBreached,
                slaDeadline,
                openRelatedCasesCount
            );
        }
    }
}

