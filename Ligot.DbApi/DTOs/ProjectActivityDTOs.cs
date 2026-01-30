using System.ComponentModel.DataAnnotations;

namespace Ligot.DbApi.DTOs;

public record ProjectActivityDto(
    int Id,
    int ProjectId,
    DateTime ActivityDate,
    string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    string? ProjectName);

public record ProjectActivityCreateDto(
    int ProjectId,
    [Required] DateTime ActivityDate,
    [Required, MaxLength(500)] string Summary,
    [MaxLength(5000)] string? Description,
    [MaxLength(1000)] string? NextAction,
    [MaxLength(100)] string? ActivityType,
    [MaxLength(200)] string? PerformedBy);

public record UpdateProjectActivityDto(
    [Required] int Id,
    [Required] int ProjectId,
    DateTime ActivityDate,
    [Required, MaxLength(500)] string Summary,
    [MaxLength(5000)] string? Description,
    [MaxLength(1000)] string? NextAction,
    [MaxLength(100)] string? ActivityType,
    [MaxLength(200)] string? PerformedBy);

