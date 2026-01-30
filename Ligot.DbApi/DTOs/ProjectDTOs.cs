using System.ComponentModel.DataAnnotations;
using Ligot.DbApi.Models;

namespace Ligot.DbApi.DTOs;

public record ProjectDto(
    int Id,
    string Name,
    string? Description,
    int CustomerId,
    string? CustomerName,
    int? CustomerOrderId,
    string? CustomerOrderNumber,
    ProjectStatus Status,
    string? ProjectReader,
    DateTime CreatedAt);

public record CreateProjectDto(
    [Required] string Name,
    string? Description,
    [Required] int CustomerId,
    int? CustomerOrderId = null,
    ProjectStatus Status = ProjectStatus.Wip,
    [MaxLength(200)] string? ProjectReader = null);

