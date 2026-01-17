using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

public record ProjectDto(
    int Id,
    string Name,
    string? Description,
    int CustomerId,
    string? CustomerName,
    int? CustomerOrderId,
    string? CustomerOrderNumber,
    ProjectStatus Status,
    DateTime CreatedAt);

public record CreateProjectDto(
    [Required] string Name,
    string? Description,
    [Required] int CustomerId,
    int? CustomerOrderId = null,
    ProjectStatus Status = ProjectStatus.Wip);
