using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Policy = "SupportOnly")] // Commented for local development
public class CaseTemplatesController : AuditableControllerBase
{
    private readonly ICaseTemplateRepository _repo;
    private readonly IAuditService _auditService;
    
    public CaseTemplatesController(
        ICaseTemplateRepository repo, 
        IUserRepository userRepo, 
        IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
    }

    [HttpGet("active")]
    public async Task<IEnumerable<CaseTemplateDto>> GetActive()
    {
        var templates = await _repo.GetAllActiveAsync();
        return templates.Select(t => new CaseTemplateDto(
            t.Id,
            t.Name,
            t.IssueType,
            t.DefaultPriority,
            t.TitleTemplate,
            t.DescriptionTemplate,
            t.IsActive,
            t.DisplayOrder,
            t.CreatedAt,
            t.UpdatedAt
        ));
    }

    [HttpGet]
    public async Task<IEnumerable<CaseTemplateDto>> GetAll()
    {
        var templates = await _repo.GetAllAsync();
        return templates.Select(t => new CaseTemplateDto(
            t.Id,
            t.Name,
            t.IssueType,
            t.DefaultPriority,
            t.TitleTemplate,
            t.DescriptionTemplate,
            t.IsActive,
            t.DisplayOrder,
            t.CreatedAt,
            t.UpdatedAt
        ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CaseTemplateDto>> Get(int id)
    {
        var t = await _repo.GetAsync(id);
        if (t == null) return NotFound();
        
        return new CaseTemplateDto(
            t.Id,
            t.Name,
            t.IssueType,
            t.DefaultPriority,
            t.TitleTemplate,
            t.DescriptionTemplate,
            t.IsActive,
            t.DisplayOrder,
            t.CreatedAt,
            t.UpdatedAt
        );
    }

    [HttpPost]
    public async Task<ActionResult<CaseTemplateDto>> Post(CreateCaseTemplateDto dto)
    {
        var template = new CaseTemplate
        {
            Name = dto.Name,
            IssueType = dto.IssueType,
            DefaultPriority = dto.DefaultPriority,
            TitleTemplate = dto.TitleTemplate,
            DescriptionTemplate = dto.DescriptionTemplate,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder
        };
        
        var created = await _repo.AddAsync(template);
        
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "CaseTemplate", created.Id, created);
        
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new CaseTemplateDto(
            created.Id,
            created.Name,
            created.IssueType,
            created.DefaultPriority,
            created.TitleTemplate,
            created.DescriptionTemplate,
            created.IsActive,
            created.DisplayOrder,
            created.CreatedAt,
            created.UpdatedAt
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdateCaseTemplateDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var existing = await _repo.GetAsync(id);
        if (existing == null) return NotFound();
        
        existing.Name = dto.Name;
        existing.IssueType = dto.IssueType;
        existing.DefaultPriority = dto.DefaultPriority;
        existing.TitleTemplate = dto.TitleTemplate;
        existing.DescriptionTemplate = dto.DescriptionTemplate;
        existing.IsActive = dto.IsActive;
        existing.DisplayOrder = dto.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _repo.UpdateAsync(existing);
        
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "CaseTemplate", id, existing);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var template = await _repo.GetAsync(id);
        await _repo.DeleteAsync(id);
        
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "CaseTemplate", id, template);
        
        return NoContent();
    }
}
