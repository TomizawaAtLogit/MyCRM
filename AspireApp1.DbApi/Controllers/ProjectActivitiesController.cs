using System.Linq;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectActivitiesController : AuditableControllerBase
{
    private readonly IProjectActivityRepository _repo;
    private readonly IAuditService _auditService;

    public ProjectActivitiesController(IProjectActivityRepository repo, IUserRepository userRepo, IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectActivityDto>> Get(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? activityType)
    {
        // If any filter is provided, use search; otherwise return all
        if (startDate.HasValue || endDate.HasValue || !string.IsNullOrWhiteSpace(activityType))
        {
            var searchResults = await _repo.SearchAsync(startDate, endDate, activityType);
            return searchResults.Select(ToDto);
        }
        
        var allActivities = await _repo.GetAllAsync();
        return allActivities.Select(ToDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectActivityDto>> Get(int id)
    {
        var activity = await _repo.GetAsync(id);
        if (activity == null)
            return NotFound();

        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "ProjectActivity", activity.Id, activity);

        return ToDto(activity);
    }

    [HttpGet("by-project/{projectId}")]
    public async Task<IEnumerable<ProjectActivityDto>> GetByProject(int projectId)
    {
        var activities = await _repo.GetByProjectIdAsync(projectId);
        var (username, userId) = await GetCurrentUserInfoAsync();
        var arr = activities.ToArray();
        await _auditService.LogActionAsync(username, userId, "Read", "ProjectActivity", projectId, new { ProjectId = projectId, Count = arr.Length });
        return arr.Select(ToDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectActivityDto>> Post(ProjectActivityCreateDto dto)
    {
        var activity = new ProjectActivity
        {
            ProjectId = dto.ProjectId,
            ActivityDate = dto.ActivityDate.Kind == DateTimeKind.Utc ? dto.ActivityDate : dto.ActivityDate.ToUniversalTime(),
            Summary = dto.Summary,
            Description = dto.Description,
            NextAction = dto.NextAction,
            ActivityType = dto.ActivityType,
            PerformedBy = dto.PerformedBy
        };
        
        var created = await _repo.AddAsync(activity);
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "ProjectActivity", created.Id, created);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdateProjectActivityDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var existing = await _repo.GetAsync(id);
        if (existing == null)
            return NotFound();

        existing.ProjectId = dto.ProjectId;
        existing.ActivityDate = dto.ActivityDate.Kind == DateTimeKind.Utc ? dto.ActivityDate : dto.ActivityDate.ToUniversalTime();
        existing.Summary = dto.Summary;
        existing.Description = dto.Description;
        existing.NextAction = dto.NextAction;
        existing.ActivityType = dto.ActivityType;
        existing.PerformedBy = dto.PerformedBy;

        await _repo.UpdateAsync(existing);

        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "ProjectActivity", id, existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repo.GetAsync(id);
        await _repo.DeleteAsync(id);
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "ProjectActivity", id, existing);
        return NoContent();
    }

    private static ProjectActivityDto ToDto(ProjectActivity activity)
    {
        return new ProjectActivityDto(
            activity.Id,
            activity.ProjectId,
            activity.ActivityDate,
            activity.Summary,
            activity.Description,
            activity.NextAction,
            activity.ActivityType,
            activity.PerformedBy,
            activity.Project?.Name);
    }
}
