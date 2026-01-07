using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

public record CaseDto(
    int Id,
    string Title,
    string? Description,
    int CustomerId,
    string? CustomerName,
    CaseStatus Status,
    CasePriority Priority,
    IssueType IssueType,
    int? AssignedToUserId,
    string? AssignedToUserName,
    string? ResolutionNotes,
    DateTime? DueDate,
    DateTime? ResolvedAt,
    DateTime? ClosedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? FirstResponseAt,
    DateTime? SlaDeadline);

public record CreateCaseDto(
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    CaseStatus Status = CaseStatus.Open,
    CasePriority Priority = CasePriority.Medium,
    IssueType IssueType = IssueType.Question,
    int? AssignedToUserId = null,
    string? ResolutionNotes = null,
    DateTime? DueDate = null);

public record UpdateCaseDto(
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    CaseStatus Status,
    CasePriority Priority,
    IssueType IssueType,
    int? AssignedToUserId,
    string? ResolutionNotes,
    DateTime? DueDate);
