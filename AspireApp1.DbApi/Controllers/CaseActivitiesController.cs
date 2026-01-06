using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseActivitiesController : AuditableControllerBase
    {
        private readonly ICaseActivityRepository _repo;
        private readonly IAuditService _auditService;
        
        public CaseActivitiesController(
            ICaseActivityRepository repo, 
            IUserRepository userRepo, 
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _auditService = auditService;
        }

        [HttpGet("case/{caseId}")]
        public async Task<IEnumerable<CaseActivityDto>> GetByCaseId(int caseId)
        {
            var activities = await _repo.GetByCaseIdAsync(caseId);
            return activities.Select(a => new CaseActivityDto(
                a.Id,
                a.CaseId,
                a.ActivityDate,
                a.Summary,
                a.Description,
                a.NextAction,
                a.ActivityType,
                a.PerformedBy,
                a.PreviousAssignedToUserId,
                a.NewAssignedToUserId,
                a.CreatedAt
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaseActivityDto>> Get(int id)
        {
            var a = await _repo.GetAsync(id);
            if (a == null) return NotFound();
            
            return new CaseActivityDto(
                a.Id,
                a.CaseId,
                a.ActivityDate,
                a.Summary,
                a.Description,
                a.NextAction,
                a.ActivityType,
                a.PerformedBy,
                a.PreviousAssignedToUserId,
                a.NewAssignedToUserId,
                a.CreatedAt
            );
        }

        [HttpPost]
        public async Task<ActionResult<CaseActivityDto>> Post(CreateCaseActivityDto dto)
        {
            var activity = new CaseActivity
            {
                CaseId = dto.CaseId,
                ActivityDate = dto.ActivityDate,
                Summary = dto.Summary,
                Description = dto.Description,
                NextAction = dto.NextAction,
                ActivityType = dto.ActivityType,
                PerformedBy = dto.PerformedBy,
                PreviousAssignedToUserId = dto.PreviousAssignedToUserId,
                NewAssignedToUserId = dto.NewAssignedToUserId
            };
            
            var created = await _repo.AddAsync(activity);
            
            // Log create action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "CaseActivity", created.Id, created);
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new CaseActivityDto(
                created.Id,
                created.CaseId,
                created.ActivityDate,
                created.Summary,
                created.Description,
                created.NextAction,
                created.ActivityType,
                created.PerformedBy,
                created.PreviousAssignedToUserId,
                created.NewAssignedToUserId,
                created.CreatedAt
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CaseActivityDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            existing.ActivityDate = dto.ActivityDate;
            existing.Summary = dto.Summary;
            existing.Description = dto.Description;
            existing.NextAction = dto.NextAction;
            existing.ActivityType = dto.ActivityType;
            existing.PerformedBy = dto.PerformedBy;
            existing.UpdatedAt = DateTime.UtcNow;
            
            await _repo.UpdateAsync(existing);
            
            // Log update action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "CaseActivity", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _repo.GetAsync(id);
            
            await _repo.DeleteAsync(id);
            
            // Log delete action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "CaseActivity", id, activity);
            
            return NoContent();
        }
    }
}
