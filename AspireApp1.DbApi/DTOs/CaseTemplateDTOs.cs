using System.ComponentModel.DataAnnotations;
using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.DTOs;

public record CaseTemplateDto(
    int Id,
    string Name,
    IssueType IssueType,
    CasePriority DefaultPriority,
    string TitleTemplate,
    string? DescriptionTemplate,
    bool IsActive,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateCaseTemplateDto(
    [Required] string Name,
    IssueType IssueType,
    CasePriority DefaultPriority,
    [Required] string TitleTemplate,
    string? DescriptionTemplate,
    bool IsActive = true,
    int DisplayOrder = 0);

public record UpdateCaseTemplateDto(
    [Required] int Id,
    [Required] string Name,
    IssueType IssueType,
    CasePriority DefaultPriority,
    [Required] string TitleTemplate,
    string? DescriptionTemplate,
    bool IsActive,
    int DisplayOrder);
