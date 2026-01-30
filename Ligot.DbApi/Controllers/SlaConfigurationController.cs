using Ligot.DbApi.Models;
using Ligot.DbApi.Repositories;
using Ligot.DbApi.DTOs;
using Ligot.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ligot.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Policy = "SupportOnly")] // Commented for local development
public class SlaConfigurationController : AuditableControllerBase
{
    private readonly ISlaConfigurationRepository _repo;
    private readonly IAuditService _auditService;
    
    public SlaConfigurationController(
        ISlaConfigurationRepository repo, 
        IUserRepository userRepo, 
        IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IEnumerable<SlaConfigurationDto>> GetAll()
    {
        var configs = await _repo.GetAllAsync();
        return configs.Select(c => new SlaConfigurationDto(
            c.Id,
            c.Priority,
            c.ResponseTimeHours,
            c.ResolutionTimeHours,
            c.IsActive,
            c.CreatedAt,
            c.UpdatedAt
        ));
    }

    [HttpPut("bulk")]
    public async Task<IActionResult> UpdateAll(UpdateAllSlaConfigurationsDto dto)
    {
        var (username, userId) = await GetCurrentUserInfoAsync();
        
        foreach (var configDto in dto.Configurations)
        {
            var existing = await _repo.GetByPriorityAsync(configDto.Priority);
            if (existing != null)
            {
                existing.ResponseTimeHours = configDto.ResponseTimeHours;
                existing.ResolutionTimeHours = configDto.ResolutionTimeHours;
                existing.UpdatedAt = DateTime.UtcNow;
                
                await _repo.UpdateAsync(existing);
                await _auditService.LogActionAsync(username, userId, "Update", "SlaConfiguration", existing.Id, existing);
            }
            else
            {
                var newConfig = new SlaThreshold
                {
                    Priority = configDto.Priority,
                    ResponseTimeHours = configDto.ResponseTimeHours,
                    ResolutionTimeHours = configDto.ResolutionTimeHours,
                    IsActive = true
                };
                
                var created = await _repo.AddAsync(newConfig);
                await _auditService.LogActionAsync(username, userId, "Create", "SlaConfiguration", created.Id, created);
            }
        }
        
        return NoContent();
    }
}

