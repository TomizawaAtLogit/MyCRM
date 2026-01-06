using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : AuditableControllerBase
    {
        private readonly ICaseRepository _repo;
        private readonly ICaseActivityRepository _activityRepo;
        private readonly IAuditService _auditService;
        
        public CasesController(
            ICaseRepository repo, 
            ICaseActivityRepository activityRepo,
            IUserRepository userRepo, 
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<CaseDto>> Get(
            [FromQuery] int? customerId, 
            [FromQuery] CaseStatus? status,
            [FromQuery] int? assignedToUserId,
            [FromQuery] CasePriority? priority)
        {
            IEnumerable<Case> cases;
            
            if (customerId.HasValue)
            {
                cases = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else if (assignedToUserId.HasValue)
            {
                cases = await _repo.GetByAssignedUserIdAsync(assignedToUserId.Value);
            }
            else if (status.HasValue)
            {
                cases = await _repo.GetByStatusAsync(status.Value);
            }
            else
            {
                cases = await _repo.GetAllAsync();
            }

            // Apply additional filters
            if (status.HasValue && !customerId.HasValue && !assignedToUserId.HasValue)
            {
                cases = cases.Where(c => c.Status == status.Value);
            }
            if (priority.HasValue)
            {
                cases = cases.Where(c => c.Priority == priority.Value);
            }

            return cases.Select(c => new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.CustomerId,
                c.Customer?.Name,
                c.Status,
                c.Priority,
                c.IssueType,
                c.AssignedToUserId,
                c.AssignedToUser?.DisplayName,
                c.ResolutionNotes,
                c.DueDate,
                c.ResolvedAt,
                c.ClosedAt,
                c.CreatedAt,
                c.UpdatedAt,
                c.FirstResponseAt,
                c.SlaDeadline
            ));
        }

        [HttpGet("overdue")]
        public async Task<IEnumerable<CaseDto>> GetOverdue()
        {
            var cases = await _repo.GetOverdueCasesAsync();
            
            return cases.Select(c => new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.CustomerId,
                c.Customer?.Name,
                c.Status,
                c.Priority,
                c.IssueType,
                c.AssignedToUserId,
                c.AssignedToUser?.DisplayName,
                c.ResolutionNotes,
                c.DueDate,
                c.ResolvedAt,
                c.ClosedAt,
                c.CreatedAt,
                c.UpdatedAt,
                c.FirstResponseAt,
                c.SlaDeadline
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaseDto>> Get(int id)
        {
            var c = await _repo.GetAsync(id);
            if (c == null) return NotFound();
            
            // Log read action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "Case", id, c);
            
            return new CaseDto(
                c.Id,
                c.Title,
                c.Description,
                c.CustomerId,
                c.Customer?.Name,
                c.Status,
                c.Priority,
                c.IssueType,
                c.AssignedToUserId,
                c.AssignedToUser?.DisplayName,
                c.ResolutionNotes,
                c.DueDate,
                c.ResolvedAt,
                c.ClosedAt,
                c.CreatedAt,
                c.UpdatedAt,
                c.FirstResponseAt,
                c.SlaDeadline
            );
        }

        [HttpPost]
        public async Task<ActionResult<CaseDto>> Post(CreateCaseDto dto)
        {
            var caseEntity = new Case
            {
                Title = dto.Title,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                Status = dto.Status,
                Priority = dto.Priority,
                IssueType = dto.IssueType,
                AssignedToUserId = dto.AssignedToUserId,
                ResolutionNotes = dto.ResolutionNotes,
                DueDate = dto.DueDate
            };
            
            var created = await _repo.AddAsync(caseEntity);
            
            // Log create action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "Case", created.Id, created);
            
            // Fetch with related data
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new CaseDto(
                result.Id,
                result.Title,
                result.Description,
                result.CustomerId,
                result.Customer?.Name,
                result.Status,
                result.Priority,
                result.IssueType,
                result.AssignedToUserId,
                result.AssignedToUser?.DisplayName,
                result.ResolutionNotes,
                result.DueDate,
                result.ResolvedAt,
                result.ClosedAt,
                result.CreatedAt,
                result.UpdatedAt,
                result.FirstResponseAt,
                result.SlaDeadline
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateCaseDto dto)
        {
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            var oldAssignedToUserId = existing.AssignedToUserId;
            
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.CustomerId = dto.CustomerId;
            existing.Status = dto.Status;
            existing.Priority = dto.Priority;
            existing.IssueType = dto.IssueType;
            existing.AssignedToUserId = dto.AssignedToUserId;
            existing.ResolutionNotes = dto.ResolutionNotes;
            existing.DueDate = dto.DueDate;
            existing.UpdatedAt = DateTime.UtcNow;
            
            // Update resolved/closed timestamps
            if (dto.Status == CaseStatus.Resolved && existing.ResolvedAt == null)
            {
                existing.ResolvedAt = DateTime.UtcNow;
            }
            if (dto.Status == CaseStatus.Closed && existing.ClosedAt == null)
            {
                existing.ClosedAt = DateTime.UtcNow;
            }
            
            await _repo.UpdateAsync(existing);
            
            // If assignment changed, create activity entry
            if (oldAssignedToUserId != dto.AssignedToUserId)
            {
                var (username, userId) = await GetCurrentUserInfoAsync();
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
            
            // Log update action
            var (auditUsername, auditUserId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(auditUsername, auditUserId, "Update", "Case", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get case before deletion for audit
            var caseEntity = await _repo.GetAsync(id);
            
            await _repo.DeleteAsync(id);
            
            // Log delete action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "Case", id, caseEntity);
            
            return NoContent();
        }
    }
}
