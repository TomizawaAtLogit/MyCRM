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
public class CaseRelationshipsController : AuditableControllerBase
{
    private readonly ICaseRelationshipRepository _repo;
    private readonly IAuditService _auditService;
    
    public CaseRelationshipsController(
        ICaseRelationshipRepository repo, 
        IUserRepository userRepo, 
        IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
    }

    [HttpGet("case/{caseId}")]
    public async Task<IEnumerable<CaseRelationshipDto>> GetByCaseId(int caseId)
    {
        var relationships = await _repo.GetByCaseIdAsync(caseId);
        return relationships.Select(r => new CaseRelationshipDto(
            r.Id,
            r.SourceCaseId,
            r.SourceCase?.Title,
            r.RelatedCaseId,
            r.RelatedCase?.Title,
            r.RelationshipType,
            r.Notes,
            r.CreatedBy,
            r.CreatedAt
        ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CaseRelationshipDto>> Get(int id)
    {
        var r = await _repo.GetAsync(id);
        if (r == null) return NotFound();
        
        return new CaseRelationshipDto(
            r.Id,
            r.SourceCaseId,
            r.SourceCase?.Title,
            r.RelatedCaseId,
            r.RelatedCase?.Title,
            r.RelationshipType,
            r.Notes,
            r.CreatedBy,
            r.CreatedAt
        );
    }

    [HttpPost]
    public async Task<ActionResult<CaseRelationshipDto>> Post(CreateCaseRelationshipDto dto)
    {
        var (username, userId) = await GetCurrentUserInfoAsync();
        
        // Create the primary relationship
        var relationship = new CaseRelationship
        {
            SourceCaseId = dto.SourceCaseId,
            RelatedCaseId = dto.RelatedCaseId,
            RelationshipType = dto.RelationshipType,
            Notes = dto.Notes,
            CreatedBy = username
        };
        
        var created = await _repo.AddAsync(relationship);
        await _auditService.LogActionAsync(username, userId, "Create", "CaseRelationship", created.Id, created);
        
        // Create bidirectional relationship
        var reverseRelationship = new CaseRelationship
        {
            SourceCaseId = dto.RelatedCaseId,
            RelatedCaseId = dto.SourceCaseId,
            RelationshipType = dto.RelationshipType,
            Notes = dto.Notes,
            CreatedBy = username
        };
        
        var createdReverse = await _repo.AddAsync(reverseRelationship);
        await _auditService.LogActionAsync(username, userId, "Create", "CaseRelationship", createdReverse.Id, createdReverse);
        
        var result = await _repo.GetAsync(created.Id);
        if (result == null) return NotFound();
        
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new CaseRelationshipDto(
            result.Id,
            result.SourceCaseId,
            result.SourceCase?.Title,
            result.RelatedCaseId,
            result.RelatedCase?.Title,
            result.RelationshipType,
            result.Notes,
            result.CreatedBy,
            result.CreatedAt
        ));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var relationship = await _repo.GetAsync(id);
        if (relationship == null) return NotFound();
        
        var (username, userId) = await GetCurrentUserInfoAsync();
        
        // Delete bidirectional relationships
        await _repo.DeleteBidirectionalAsync(
            relationship.SourceCaseId, 
            relationship.RelatedCaseId, 
            relationship.RelationshipType);
        
        await _auditService.LogActionAsync(username, userId, "Delete", "CaseRelationship", id, relationship);
        
        return NoContent();
    }
}

