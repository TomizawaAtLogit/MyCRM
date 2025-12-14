using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
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
        public async Task<IEnumerable<Project>> Get() => await _repo.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> Get(int id)
        {
            var p = await _repo.GetAsync(id);
            if (p == null) return NotFound();
            return p;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> Post(Project project)
        {
            var created = await _repo.AddAsync(project);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
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
