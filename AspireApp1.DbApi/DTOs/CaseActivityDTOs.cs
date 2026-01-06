using System.ComponentModel.DataAnnotations;

namespace AspireApp1.DbApi.DTOs;

public record CaseActivityDto(
    int Id,
    int CaseId,
    DateTime ActivityDate,
    string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    int? PreviousAssignedToUserId,
    int? NewAssignedToUserId,
    DateTime? CreatedAt);

public record CreateCaseActivityDto(
    [Required] int CaseId,
    [Required] DateTime ActivityDate,
    [Required] string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    int? PreviousAssignedToUserId = null,
    int? NewAssignedToUserId = null);
