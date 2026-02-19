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
    // [Authorize(Policy = "PreSalesOnly")] // Commented for local development
    public class ProjectsController : AuditableControllerBase
    {
        private readonly IProjectRepository _repo;
        private readonly IOrderRepository _orderRepo;
        private readonly IAuditService _auditService;
        
        public ProjectsController(IProjectRepository repo, IOrderRepository orderRepo, IUserRepository userRepo, IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectDto>> Get([FromQuery] int? customerId, [FromQuery] ProjectStatus? status, [FromQuery] string? projectReader)
        {
            IEnumerable<Project> projects;
            
            // Get current user and their allowed customer IDs
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            if (!userId.HasValue)
            {
                // If user is not found, return empty list
                return Enumerable.Empty<ProjectDto>();
            }
            
            var allowedCustomerIds = await _userRepo.GetAllowedCustomerIdsAsync(userId.Value);
            
            if (customerId.HasValue)
            {
                // Check if the requested customer is allowed
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0 && !allowedCustomerIds.Contains(customerId.Value))
                {
                    // User doesn't have access to this customer
                    return Enumerable.Empty<ProjectDto>();
                }
                projects = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else
            {
                projects = await _repo.GetAllAsync(allowedCustomerIds);
            }

            // Apply status filter if provided
            if (status.HasValue)
            {
                projects = projects.Where(p => p.Status == status.Value);
            }

            // Apply projectReader filter if provided
            if (!string.IsNullOrEmpty(projectReader))
            {
                projects = projects.Where(p => !string.IsNullOrEmpty(p.ProjectReader) && p.ProjectReader == projectReader);
            }

            return projects.Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.CustomerId,
                p.Customer?.Name,
                p.CustomerOrderId,
                p.CustomerOrder?.OrderNumber,
                p.Status,
                p.ProjectReader,
                p.CreatedAt
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> Get(int id)
        {
            var p = await _repo.GetAsync(id);
            if (p == null) return NotFound();
            
            // Log read action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "Project", id, p);
            
            return new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.CustomerId,
                p.Customer?.Name,
                p.CustomerOrderId,
                p.CustomerOrder?.OrderNumber,
                p.Status,
                p.ProjectReader,
                p.CreatedAt
            );
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Post(CreateProjectDto dto)
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
            
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                CustomerOrderId = dto.CustomerOrderId,
                Status = dto.Status,
                ProjectReader = dto.ProjectReader
            };
            
            var created = await _repo.AddAsync(project);
            
            // Log create action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "Project", created.Id, created);
            
            // Fetch with customer info
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new ProjectDto(
                result.Id,
                result.Name,
                result.Description,
                result.CustomerId,
                result.Customer?.Name,
                result.CustomerOrderId,
                result.CustomerOrder?.OrderNumber,
                result.Status,
                result.ProjectReader,
                result.CreatedAt
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CreateProjectDto dto)
        {
            var existing = await _repo.GetAsync(id);
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
            
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CustomerId = dto.CustomerId;
            existing.CustomerOrderId = dto.CustomerOrderId;
            existing.Status = dto.Status;
            existing.ProjectReader = dto.ProjectReader;
            
            await _repo.UpdateAsync(existing);
            
            // Log update action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "Project", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get project before deletion for audit
            var project = await _repo.GetAsync(id);
            
            await _repo.DeleteAsync(id);
            
            // Log delete action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "Project", id, project);
            
            return NoContent();
        }
    }
}
