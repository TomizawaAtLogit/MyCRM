using System.ComponentModel.DataAnnotations;
using Ligot.DbApi.Models;

namespace Ligot.DbApi.DTOs;

public record CaseDto(
    int Id,
    string Title,
    string? Description,
    int CustomerId,
    string? CustomerName,
    int? SystemId,
    string? SystemName,
    int? SystemComponentId,
    string? SystemComponentName,
    int? CustomerSiteId,
    string? CustomerSiteName,
    int? CustomerOrderId,
    string? CustomerOrderName,
    CaseStatus Status,
    CasePriority Priority,
    IssueType IssueType,
    int? AssignedToUserId,
    string? AssignedToUserName,
    string? ResolutionNotes,
    DateTime? DueDate,
    DateTime? FirstResponseAt,
    DateTime? ResolvedAt,
    DateTime? ClosedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? ResponseTimeSlaMinutes,
    int? ResolutionTimeSlaMinutes,
    bool IsResponseSlaBreached,
    bool IsResolutionSlaBreached,
    DateTime? SlaDeadline,
    int OpenRelatedCasesCount);

public record CreateCaseDto(
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    int? SystemId,
    int? SystemComponentId,
    int? CustomerSiteId,
    int? CustomerOrderId,
    CaseStatus Status = CaseStatus.Open,
    CasePriority Priority = CasePriority.Medium,
    IssueType IssueType = IssueType.Question,
    int? AssignedToUserId = null,
    string? ResolutionNotes = null,
    DateTime? DueDate = null);

public record UpdateCaseDto(
    [Required] int Id,
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    int? SystemId,
    int? SystemComponentId,
    int? CustomerSiteId,
    int? CustomerOrderId,
    CaseStatus Status,
    CasePriority Priority,
    IssueType IssueType,
    int? AssignedToUserId,
    string? ResolutionNotes,
    DateTime? DueDate);

public record BulkUpdateCasesDto(
    [Required] int[] CaseIds,
    int? AssignedToUserId,
    CaseStatus? Status);

public record BulkUpdateCasesResponse(
    int UpdatedCount,
    int[] UpdatedCaseIds,
    bool HasWarning,
    string? WarningMessage);


