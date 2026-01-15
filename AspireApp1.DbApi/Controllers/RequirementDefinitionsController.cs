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
    public class RequirementDefinitionsController : AuditableControllerBase
    {
        private readonly IRequirementDefinitionRepository _repo;
        private readonly IAuditService _auditService;
        
        public RequirementDefinitionsController(
            IRequirementDefinitionRepository repo,
            IUserRepository userRepo,
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<RequirementDefinitionDto>> Get([FromQuery] int? customerId)
        {
            IEnumerable<RequirementDefinition> entities;
            
            if (customerId.HasValue)
            {
                entities = await _repo.GetByCustomerIdAsync(customerId.Value);
            }
            else
            {
                entities = await _repo.GetAllAsync();
            }

            return entities.Select(r => new RequirementDefinitionDto(
                r.Id,
                r.Title,
                r.Description,
                r.CustomerId,
                r.Customer?.Name,
                r.Category,
                r.Priority,
                r.Status,
                r.CreatedAt,
                r.UpdatedAt));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementDefinitionDto>> Get(int id)
        {
            var entity = await _repo.GetAsync(id);
            if (entity == null) return NotFound();
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "RequirementDefinition", id, entity);
            
            return new RequirementDefinitionDto(
                entity.Id,
                entity.Title,
                entity.Description,
                entity.CustomerId,
                entity.Customer?.Name,
                entity.Category,
                entity.Priority,
                entity.Status,
                entity.CreatedAt,
                entity.UpdatedAt);
        }

        [HttpPost]
        public async Task<ActionResult<RequirementDefinitionDto>> Post(CreateRequirementDefinitionDto dto)
        {
            var entity = new RequirementDefinition
            {
                Title = dto.Title,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                Category = dto.Category,
                Priority = dto.Priority,
                Status = dto.Status
            };
            
            var created = await _repo.AddAsync(entity);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "RequirementDefinition", created.Id, created);
            
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new RequirementDefinitionDto(
                result.Id,
                result.Title,
                result.Description,
                result.CustomerId,
                result.Customer?.Name,
                result.Category,
                result.Priority,
                result.Status,
                result.CreatedAt,
                result.UpdatedAt));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateRequirementDefinitionDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.CustomerId = dto.CustomerId;
            existing.Category = dto.Category;
            existing.Priority = dto.Priority;
            existing.Status = dto.Status;
            
            await _repo.UpdateAsync(existing);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "RequirementDefinition", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repo.GetAsync(id);
            await _repo.DeleteAsync(id);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "RequirementDefinition", id, entity);
            
            return NoContent();
        }
    }
}
