using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _repo;
        public ProjectsController(IProjectRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IEnumerable<ProjectDto>> Get([FromQuery] int? customerId, [FromQuery] ProjectStatus? status)
        {
            IEnumerable<Project> projects;
            
            if (customerId.HasValue)
            {
                projects = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else
            {
                projects = await _repo.GetAllAsync();
            }

            // Apply status filter if provided
            if (status.HasValue)
            {
                projects = projects.Where(p => p.Status == status.Value);
            }

            return projects.Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.CustomerId,
                p.Customer?.Name,
                p.Status,
                p.CreatedAt
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> Get(int id)
        {
            var p = await _repo.GetAsync(id);
            if (p == null) return NotFound();
            
            return new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.CustomerId,
                p.Customer?.Name,
                p.Status,
                p.CreatedAt
            );
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Post(CreateProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                Status = dto.Status
            };
            
            var created = await _repo.AddAsync(project);
            
            // Fetch with customer info
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new ProjectDto(
                result.Id,
                result.Name,
                result.Description,
                result.CustomerId,
                result.Customer?.Name,
                result.Status,
                result.CreatedAt
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Project project)
        {
            if (id != project.Id) return BadRequest();
            await _repo.UpdateAsync(project);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
