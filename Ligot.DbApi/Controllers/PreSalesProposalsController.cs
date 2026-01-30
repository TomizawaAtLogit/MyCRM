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
    // [Authorize] // Commented for local development
    public class PreSalesProposalsController : AuditableControllerBase
    {
        private readonly IPreSalesProposalRepository _repo;
        private readonly IPreSalesActivityRepository _activityRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IAuditService _auditService;
        
        public PreSalesProposalsController(
            IPreSalesProposalRepository repo,
            IPreSalesActivityRepository activityRepo,
            IOrderRepository orderRepo,
            IUserRepository userRepo,
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
            _orderRepo = orderRepo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<PreSalesProposalDto>> Get(
            [FromQuery] int? customerId,
            [FromQuery] int? assignedToUserId,
            [FromQuery] PreSalesStatus? status,
            [FromQuery] PreSalesStage? stage)
        {
            IEnumerable<PreSalesProposal> proposals;
            
            // Get current user and their allowed customer IDs
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            if (!userId.HasValue)
            {
                // If user is not found, return empty list
                return Enumerable.Empty<PreSalesProposalDto>();
            }
            
            var allowedCustomerIds = await _userRepo.GetAllowedCustomerIdsAsync(userId.Value);
            
            if (customerId.HasValue)
            {
                // Check if the requested customer is allowed
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0 && !allowedCustomerIds.Contains(customerId.Value))
                {
                    // User doesn't have access to this customer
                    return Enumerable.Empty<PreSalesProposalDto>();
                }
                proposals = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else if (assignedToUserId.HasValue)
            {
                proposals = await _repo.GetByAssignedUserIdAsync(assignedToUserId.Value);
                // Apply coverage filter after getting by assigned user
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    proposals = proposals.Where(p => allowedCustomerIds.Contains(p.CustomerId));
                }
            }
            else if (status.HasValue)
            {
                proposals = await _repo.GetByStatusAsync(status.Value);
                // Apply coverage filter after getting by status
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    proposals = proposals.Where(p => allowedCustomerIds.Contains(p.CustomerId));
                }
            }
            else if (stage.HasValue)
            {
                proposals = await _repo.GetByStageAsync(stage.Value);
                // Apply coverage filter after getting by stage
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    proposals = proposals.Where(p => allowedCustomerIds.Contains(p.CustomerId));
                }
            }
            else
            {
                proposals = await _repo.GetAllAsync(allowedCustomerIds);
            }

            return proposals.Select(p => BuildDto(p));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PreSalesProposalDto>> Get(int id)
        {
            var proposal = await _repo.GetAsync(id);
            if (proposal == null) return NotFound();
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "PreSalesProposal", id, proposal);
            
            return BuildDto(proposal);
        }

        [HttpPost]
        public async Task<ActionResult<PreSalesProposalDto>> Post(CreatePreSalesProposalDto dto)
        {
            // Validate CustomerOrderId belongs to CustomerId
            if (dto.CustomerOrderId.HasValue)
            {
                var order = await _orderRepo.GetAsync(dto.CustomerOrderId.Value);
                if (order == null)
                    return BadRequest("The specified CustomerOrderId does not exist.");
                if (order.CustomerId != dto.CustomerId)
                    return BadRequest("The specified CustomerOrderId does not belong to the specified CustomerId.");
            }
            
            var proposal = new PreSalesProposal
            {
                Title = dto.Title,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                RequirementDefinitionId = dto.RequirementDefinitionId,
                CustomerOrderId = dto.CustomerOrderId,
                Status = dto.Status,
                Stage = dto.Stage,
                AssignedToUserId = dto.AssignedToUserId,
                EstimatedValue = dto.EstimatedValue,
                ProbabilityPercentage = dto.ProbabilityPercentage,
                ExpectedCloseDate = dto.ExpectedCloseDate,
                Notes = dto.Notes
            };
            
            var created = await _repo.AddAsync(proposal);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "PreSalesProposal", created.Id, created);
            
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, BuildDto(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdatePreSalesProposalDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsyncForUpdateAsync(id);
            if (existing == null) return NotFound();
            
            // Validate CustomerOrderId belongs to CustomerId
            if (dto.CustomerOrderId.HasValue)
            {
                var order = await _orderRepo.GetAsync(dto.CustomerOrderId.Value);
                if (order == null)
                    return BadRequest("The specified CustomerOrderId does not exist.");
                if (order.CustomerId != dto.CustomerId)
                    return BadRequest("The specified CustomerOrderId does not belong to the specified CustomerId.");
            }
            
            var oldAssignedToUserId = existing.AssignedToUserId;
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.CustomerId = dto.CustomerId;
            existing.RequirementDefinitionId = dto.RequirementDefinitionId;
            existing.CustomerOrderId = dto.CustomerOrderId;
            existing.Status = dto.Status;
            existing.Stage = dto.Stage;
            existing.AssignedToUserId = dto.AssignedToUserId;
            existing.EstimatedValue = dto.EstimatedValue;
            existing.ProbabilityPercentage = dto.ProbabilityPercentage;
            existing.ExpectedCloseDate = dto.ExpectedCloseDate;
            existing.Notes = dto.Notes;
            
            // Auto-set ClosedAt when status is Closed
            if ((dto.Status == PreSalesStatus.Closed || dto.Stage == PreSalesStage.Won || dto.Stage == PreSalesStage.Lost) 
                && existing.ClosedAt == null)
            {
                existing.ClosedAt = DateTime.UtcNow;
            }
            
            await _repo.UpdateAsync(existing);
            
            await _auditService.LogActionAsync(username, userId, "Update", "PreSalesProposal", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var proposal = await _repo.GetAsync(id);
            await _repo.DeleteAsync(id);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "PreSalesProposal", id, proposal);
            
            return NoContent();
        }

        private static PreSalesProposalDto BuildDto(PreSalesProposal p)
        {
            return new PreSalesProposalDto(
                p.Id,
                p.Title,
                p.Description,
                p.CustomerId,
                p.Customer?.Name,
                p.RequirementDefinitionId,
                p.RequirementDefinition?.Title,
                p.CustomerOrderId,
                p.CustomerOrder?.OrderNumber,
                p.Status,
                p.Stage,
                p.AssignedToUserId,
                p.AssignedToUser?.DisplayName,
                p.EstimatedValue,
                p.ProbabilityPercentage,
                p.ExpectedCloseDate,
                p.ClosedAt,
                p.Notes,
                p.CreatedAt,
                p.UpdatedAt);
        }
    }
}

