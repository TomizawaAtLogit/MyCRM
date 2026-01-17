using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

// Requirement Definition DTOs
public record RequirementDefinitionDto(
    int Id,
    string Title,
    string? Description,
    int CustomerId,
    string? CustomerName,
    string? Category,
    string? Priority,
    string? Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateRequirementDefinitionDto(
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    string? Category = null,
    string? Priority = null,
    string? Status = null);

public record UpdateRequirementDefinitionDto(
    [Required] int Id,
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    string? Category,
    string? Priority,
    string? Status);

// Pre-sales Proposal DTOs
public record PreSalesProposalDto(
    int Id,
    string Title,
    string? Description,
    int CustomerId,
    string? CustomerName,
    int? RequirementDefinitionId,
    string? RequirementDefinitionTitle,
    int? CustomerOrderId,
    string? CustomerOrderNumber,
    PreSalesStatus Status,
    PreSalesStage Stage,
    int? AssignedToUserId,
    string? AssignedToUserName,
    decimal? EstimatedValue,
    int? ProbabilityPercentage,
    DateTime? ExpectedCloseDate,
    DateTime? ClosedAt,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreatePreSalesProposalDto(
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    int? RequirementDefinitionId = null,
    int? CustomerOrderId = null,
    PreSalesStatus Status = PreSalesStatus.Draft,
    PreSalesStage Stage = PreSalesStage.InitialContact,
    int? AssignedToUserId = null,
    decimal? EstimatedValue = null,
    int? ProbabilityPercentage = null,
    DateTime? ExpectedCloseDate = null,
    string? Notes = null);

public record UpdatePreSalesProposalDto(
    [Required] int Id,
    [Required] string Title,
    string? Description,
    [Required] int CustomerId,
    int? RequirementDefinitionId,
    int? CustomerOrderId,
    PreSalesStatus Status,
    PreSalesStage Stage,
    int? AssignedToUserId,
    decimal? EstimatedValue,
    int? ProbabilityPercentage,
    DateTime? ExpectedCloseDate,
    string? Notes);

// Pre-sales Activity DTOs
public record PreSalesActivityDto(
    int Id,
    int PreSalesProposalId,
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

public record CreatePreSalesActivityDto(
    [Required] int PreSalesProposalId,
    [Required] string Summary,
    string? Description = null,
    string? NextAction = null,
    string? ActivityType = null,
    string? PerformedBy = null,
    DateTime? ActivityDate = null);

public record UpdatePreSalesActivityDto(
    [Required] int Id,
    [Required] int PreSalesProposalId,
    [Required] string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    DateTime ActivityDate);
