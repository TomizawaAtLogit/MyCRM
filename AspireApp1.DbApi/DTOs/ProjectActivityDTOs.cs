using System.ComponentModel.DataAnnotations;

namespace AspireApp1.DbApi.DTOs;

public record ProjectActivityCreateDto(
    int ProjectId,
    [Required] DateTime ActivityDate,
    [Required, MaxLength(500)] string Summary,
    [MaxLength(5000)] string? Description,
    [MaxLength(1000)] string? NextAction,
    [MaxLength(100)] string? ActivityType,
    [MaxLength(200)] string? PerformedBy);
