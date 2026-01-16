using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspireApp1.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Commented for local development
    public class PreSalesActivitiesController : AuditableControllerBase
    {
        private readonly IPreSalesActivityRepository _repo;
        private readonly IAuditService _auditService;
        
        public PreSalesActivitiesController(
            IPreSalesActivityRepository repo,
            IUserRepository userRepo,
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<PreSalesActivityDto>> Get([FromQuery] int? proposalId)
        {
            IEnumerable<PreSalesActivity> activities;
            
            if (proposalId.HasValue)
            {
                activities = await _repo.GetByProposalIdAsync(proposalId.Value);
            }
            else
            {
                activities = await _repo.GetAllAsync();
            }

            return activities.Select(a => new PreSalesActivityDto(
                a.Id,
                a.PreSalesProposalId,
                a.ActivityDate,
                a.Summary,
                a.Description,
                a.NextAction,
                a.ActivityType,
                a.PerformedBy,
                a.PreviousAssignedToUserId,
                a.NewAssignedToUserId,
                a.CreatedAt,
                a.UpdatedAt));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PreSalesActivityDto>> Get(int id)
        {
            var activity = await _repo.GetAsync(id);
            if (activity == null) return NotFound();
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "PreSalesActivity", id, activity);
            
            return new PreSalesActivityDto(
                activity.Id,
                activity.PreSalesProposalId,
                activity.ActivityDate,
                activity.Summary,
                activity.Description,
                activity.NextAction,
                activity.ActivityType,
                activity.PerformedBy,
                activity.PreviousAssignedToUserId,
                activity.NewAssignedToUserId,
                activity.CreatedAt,
                activity.UpdatedAt);
        }

        [HttpPost]
        public async Task<ActionResult<PreSalesActivityDto>> Post(CreatePreSalesActivityDto dto)
        {
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            var activity = new PreSalesActivity
            {
                PreSalesProposalId = dto.PreSalesProposalId,
                ActivityDate = dto.ActivityDate ?? DateTime.UtcNow,
                Summary = dto.Summary,
                Description = dto.Description,
                NextAction = dto.NextAction,
                ActivityType = dto.ActivityType,
                PerformedBy = dto.PerformedBy ?? username
            };
            
            var created = await _repo.AddAsync(activity);
            
            await _auditService.LogActionAsync(username, userId, "Create", "PreSalesActivity", created.Id, created);
            
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new PreSalesActivityDto(
                result.Id,
                result.PreSalesProposalId,
                result.ActivityDate,
                result.Summary,
                result.Description,
                result.NextAction,
                result.ActivityType,
                result.PerformedBy,
                result.PreviousAssignedToUserId,
                result.NewAssignedToUserId,
                result.CreatedAt,
                result.UpdatedAt));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdatePreSalesActivityDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            existing.PreSalesProposalId = dto.PreSalesProposalId;
            existing.ActivityDate = dto.ActivityDate;
            existing.Summary = dto.Summary;
            existing.Description = dto.Description;
            existing.NextAction = dto.NextAction;
            existing.ActivityType = dto.ActivityType;
            existing.PerformedBy = dto.PerformedBy ?? existing.PerformedBy;
            
            await _repo.UpdateAsync(existing);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "PreSalesActivity", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _repo.GetAsync(id);
            await _repo.DeleteAsync(id);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "PreSalesActivity", id, activity);
            
            return NoContent();
        }
    }
}
