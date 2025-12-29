using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectActivitiesController : ControllerBase
{
    private readonly IProjectActivityRepository _repo;

    public ProjectActivitiesController(IProjectActivityRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectActivity>> Get(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? customerId,
        [FromQuery] string? activityType)
    {
        // If any filter is provided, use search; otherwise return all
        if (startDate.HasValue || endDate.HasValue || customerId.HasValue || !string.IsNullOrWhiteSpace(activityType))
        {
            return await _repo.SearchAsync(startDate, endDate, customerId, activityType);
        }
        
        return await _repo.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectActivity>> Get(int id)
    {
        var activity = await _repo.GetAsync(id);
        if (activity == null)
            return NotFound();
        return activity;
    }

    [HttpGet("by-project/{projectId}")]
    public async Task<IEnumerable<ProjectActivity>> GetByProject(int projectId)
    {
        return await _repo.GetByProjectIdAsync(projectId);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectActivity>> Post(ProjectActivity activity)
    {
        var created = await _repo.AddAsync(activity);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ProjectActivity activity)
    {
        if (id != activity.Id)
            return BadRequest();
        await _repo.UpdateAsync(activity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
