using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

public record SlaConfigurationDto(
    int Id,
    CasePriority Priority,
    int ResponseTimeHours,
    int ResolutionTimeHours,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record UpdateSlaConfigurationDto(
    [Required] CasePriority Priority,
    [Required] int ResponseTimeHours,
    [Required] int ResolutionTimeHours);

public record UpdateAllSlaConfigurationsDto(
    [Required] UpdateSlaConfigurationDto[] Configurations);
