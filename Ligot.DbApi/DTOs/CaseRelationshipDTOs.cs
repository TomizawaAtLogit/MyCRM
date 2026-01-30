using System.ComponentModel.DataAnnotations;
using Ligot.DbApi.Models;

namespace Ligot.DbApi.DTOs;

public record CaseRelationshipDto(
    int Id,
    int SourceCaseId,
    string? SourceCaseTitle,
    int RelatedCaseId,
    string? RelatedCaseTitle,
    CaseRelationshipType RelationshipType,
    string? Notes,
    string? CreatedBy,
    DateTime CreatedAt);

public record CreateCaseRelationshipDto(
    [Required] int SourceCaseId,
    [Required] int RelatedCaseId,
    CaseRelationshipType RelationshipType = CaseRelationshipType.Related,
    string? Notes = null);

