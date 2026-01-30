using System.ComponentModel.DataAnnotations;

namespace Ligot.DbApi.DTOs;

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
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateCaseActivityDto(
    [Required] int CaseId,
    DateTime? ActivityDate,
    [Required] string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy);

public record UpdateCaseActivityDto(
    [Required] int Id,
    [Required] int CaseId,
    DateTime ActivityDate,
    [Required] string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy);


