using Ligot.DbApi.Models;

namespace Ligot.DbApi.DTOs;

public record DashboardCaseItem(
    int Id,
    string Title,
    string? CustomerName,
    CasePriority Priority,
    CaseStatus Status,
    DateTime? DueDate,
    DateTime? SlaDeadline,
    bool IsResolutionSlaBreached,
    DateTime? LatestActivityDate,
    string? LatestNextAction
);

public record DashboardCaseStats(
    int MyOpenCases,
    int MyInProgressCases,
    int MyResolvedThisMonth,
    int MySlaBreachedCases
);

public record DashboardProjectItem(
    int Id,
    string Name,
    string? CustomerName,
    ProjectStatus Status,
    DateTime CreatedAt
);

public record DashboardProjectStats(
    int ActiveProjects,
    int CompletedProjects,
    int OnHoldProjects
);

public record DashboardPreSalesItem(
    int Id,
    string Title,
    string? CustomerName,
    PreSalesStatus Status,
    PreSalesStage Stage,
    int? ProbabilityPercentage,
    DateTime? ExpectedCloseDate
);

public record DashboardPreSalesStats(
    int InPipeline,
    int Won,
    int LostOrRejected
);

public record PersonalDashboardDto(
    List<DashboardCaseItem> CaseItems,
    DashboardCaseStats CaseStats,
    List<DashboardProjectItem> ProjectItems,
    DashboardProjectStats ProjectStats,
    List<DashboardPreSalesItem> PreSalesItems,
    DashboardPreSalesStats PreSalesStats
);
