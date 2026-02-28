namespace Ligot.Web;

public class DashboardApiClient
{
    private readonly HttpClient _httpClient;

    public DashboardApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardMetricDto?> GetCurrentMetricsAsync()
    {
        return await _httpClient.GetFromJsonAsync<DashboardMetricDto>("/api/dashboard/current");
    }

    public async Task<DashboardMetricDto?> GetCurrentMetricsForCustomerAsync(int customerId)
    {
        return await _httpClient.GetFromJsonAsync<DashboardMetricDto>($"/api/dashboard/current/customer/{customerId}");
    }

    public async Task<List<DashboardMetricDto>?> GetHistoricalMetricsAsync(int days = 30)
    {
        return await _httpClient.GetFromJsonAsync<List<DashboardMetricDto>>($"/api/dashboard/history?days={days}");
    }

    public async Task<List<DashboardMetricDto>?> GetHistoricalMetricsForCustomerAsync(int customerId, int days = 30)
    {
        return await _httpClient.GetFromJsonAsync<List<DashboardMetricDto>>($"/api/dashboard/history/customer/{customerId}?days={days}");
    }

    public async Task<HttpResponseMessage> GenerateSnapshotAsync()
    {
        return await _httpClient.PostAsync("/api/dashboard/snapshot", null);
    }

    public async Task<PersonalDashboardDto?> GetPersonalDashboardAsync()
    {
        return await _httpClient.GetFromJsonAsync<PersonalDashboardDto>("/api/dashboard/personal");
    }
}

public record DashboardMetricDto(
    DateTime SnapshotDate,
    int? RoleId,
    int? CustomerId,
    int TotalPreSalesProposals,
    int ActivePreSalesProposals,
    int PreSalesProposalsByStageIdentification,
    int PreSalesProposalsByStageQualification,
    int PreSalesProposalsByStageProposal,
    int PreSalesProposalsByStageNegotiation,
    int PreSalesProposalsByStageClosedWon,
    int PreSalesProposalsByStageClosedLost,
    int TotalCases,
    int OpenCases,
    int InProgressCases,
    int ResolvedCases,
    int ClosedCases,
    int CriticalPriorityCases,
    int HighPriorityCases,
    int MediumPriorityCases,
    int LowPriorityCases,
    decimal CaseResolutionRate,
    decimal SlaComplianceRate,
    decimal AverageResolutionTimeHours,
    int CasesResolvedWithinSla,
    int CasesResolvedOutsideSla,
    int TotalProjects,
    int ActiveProjects,
    int CompletedProjects,
    int OnHoldProjects,
    decimal ProjectCompletionRate
);

public record PersonalDashboardDto(
    List<DashboardCaseItem> CaseItems,
    DashboardCaseStats CaseStats,
    List<DashboardProjectItem> ProjectItems,
    DashboardProjectStats ProjectStats
);

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


