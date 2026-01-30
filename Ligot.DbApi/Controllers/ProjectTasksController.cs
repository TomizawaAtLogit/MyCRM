using Ligot.DbApi.Models;
using Ligot.DbApi.Repositories;
using Ligot.DbApi.DTOs;
using Ligot.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ligot.DbApi.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    // [Authorize(Policy = "PreSalesOnly")] // Commented for local development
    public class ProjectTasksController : AuditableControllerBase
    {
        private readonly IProjectTaskRepository _taskRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IAuditService _auditService;
        
        public ProjectTasksController(
            IProjectTaskRepository taskRepo, 
            IProjectRepository projectRepo,
            IUserRepository userRepo, 
            IAuditService auditService)
            : base(userRepo)
        {
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasks(int projectId)
        {
            // Verify project exists
            var project = await _projectRepo.GetAsync(projectId);
            if (project == null) return NotFound($"Project {projectId} not found");
            
            var tasks = await _taskRepo.GetByProjectIdAsync(projectId);
            
            return Ok(tasks.Select(t => new ProjectTaskDto(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.StartAtUtc,
                t.EndAtUtc,
                t.Status,
                t.PerformedBy,
                t.DisplayOrder,
                t.CreatedAt,
                t.UpdatedAt
            )));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskDto>> CreateTask(int projectId, CreateProjectTaskDto dto)
        {
            // Verify project exists
            var project = await _projectRepo.GetAsync(projectId);
            if (project == null) return NotFound($"Project {projectId} not found");
            
            var task = new ProjectTask
            {
                ProjectId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                StartAtUtc = dto.StartAtUtc,
                EndAtUtc = dto.EndAtUtc,
                Status = dto.Status,
                PerformedBy = dto.PerformedBy,
                DisplayOrder = dto.DisplayOrder
            };
            
            var created = await _taskRepo.AddAsync(task);
            
            // Log create action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "ProjectTask", created.Id, created);
            
            return CreatedAtAction(nameof(GetTask), new { projectId, id = created.Id }, new ProjectTaskDto(
                created.Id,
                created.ProjectId,
                created.Title,
                created.Description,
                created.StartAtUtc,
                created.EndAtUtc,
                created.Status,
                created.PerformedBy,
                created.DisplayOrder,
                created.CreatedAt,
                created.UpdatedAt
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTask(int projectId, int id)
        {
            var task = await _taskRepo.GetAsync(id);
            if (task == null || task.ProjectId != projectId) 
                return NotFound($"Task {id} not found in project {projectId}");
            
            // Log read action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "ProjectTask", id, task);
            
            return new ProjectTaskDto(
                task.Id,
                task.ProjectId,
                task.Title,
                task.Description,
                task.StartAtUtc,
                task.EndAtUtc,
                task.Status,
                task.PerformedBy,
                task.DisplayOrder,
                task.CreatedAt,
                task.UpdatedAt
            );
        }
    }
    
    [ApiController]
    [Route("api/tasks")]
    // [Authorize(Policy = "PreSalesOnly")] // Commented for local development
    public class TasksController : AuditableControllerBase
    {
        private readonly IProjectTaskRepository _taskRepo;
        private readonly IAuditService _auditService;
        
        public TasksController(
            IProjectTaskRepository taskRepo,
            IUserRepository userRepo, 
            IAuditService auditService)
            : base(userRepo)
        {
            _taskRepo = taskRepo;
            _auditService = auditService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateProjectTaskDto dto)
        {
            var existing = await _taskRepo.GetAsync(id);
            if (existing == null) return NotFound($"Task {id} not found");
            
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.StartAtUtc = dto.StartAtUtc;
            existing.EndAtUtc = dto.EndAtUtc;
            existing.Status = dto.Status;
            existing.PerformedBy = dto.PerformedBy;
            existing.DisplayOrder = dto.DisplayOrder;
            
            await _taskRepo.UpdateAsync(existing);
            
            // Log update action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "ProjectTask", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            // Get task before deletion for audit
            var task = await _taskRepo.GetAsync(id);
            if (task == null) return NotFound($"Task {id} not found");
            
            await _taskRepo.DeleteAsync(id);
            
            // Log delete action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "ProjectTask", id, task);
            
            return NoContent();
        }
    }
}

