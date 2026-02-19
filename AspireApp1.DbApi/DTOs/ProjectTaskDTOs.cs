using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

public record ProjectTaskDto(
    int Id,
    int ProjectId,
    string Title,
    string? Description,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ProjectTaskStatus Status,
    string? PerformedBy,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateProjectTaskDto(
    [Required] string Title,
    string? Description,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ProjectTaskStatus Status = ProjectTaskStatus.NotStarted,
    [MaxLength(200)] string? PerformedBy = null,
    int DisplayOrder = 0);

public record UpdateProjectTaskDto(
    [Required] string Title,
    string? Description,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ProjectTaskStatus Status,
    [MaxLength(200)] string? PerformedBy,
    int DisplayOrder);
