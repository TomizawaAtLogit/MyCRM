using Ligot.DbApi.Data;
using Ligot.DbApi.DTOs;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Services;

public interface IDashboardService
{
    Task<DashboardMetric> GetCurrentMetricsAsync(int? roleId = null, int? customerId = null);
    Task<List<DashboardMetric>> GetHistoricalMetricsAsync(int? roleId = null, int? customerId = null, int days = 30);
    Task<DashboardMetric> GenerateSnapshotAsync(int? roleId = null, int? customerId = null);
    Task<PersonalDashboardDto> GetPersonalDashboardAsync(int userId, int[]? allowedCustomerIds);
}

public class DashboardService : IDashboardService
{
    private readonly ProjectDbContext _context;

    public DashboardService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetric> GetCurrentMetricsAsync(int? roleId = null, int? customerId = null)
    {
        var metric = await CalculateMetricsAsync(roleId, customerId);
        metric.SnapshotDate = DateTime.UtcNow;
        return metric;
    }

    public async Task<List<DashboardMetric>> GetHistoricalMetricsAsync(int? roleId = null, int? customerId = null, int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        
        var query = _context.DashboardMetrics
            .Where(m => m.SnapshotDate >= cutoffDate)
            .OrderByDescending(m => m.SnapshotDate);

        if (roleId.HasValue)
            query = (IOrderedQueryable<DashboardMetric>)query.Where(m => m.RoleId == roleId.Value);

        if (customerId.HasValue)
            query = (IOrderedQueryable<DashboardMetric>)query.Where(m => m.CustomerId == customerId.Value);

        return await query.ToListAsync();
    }

    public async Task<DashboardMetric> GenerateSnapshotAsync(int? roleId = null, int? customerId = null)
    {
        var metric = await CalculateMetricsAsync(roleId, customerId);
        metric.SnapshotDate = DateTime.UtcNow;
        
        _context.DashboardMetrics.Add(metric);
        await _context.SaveChangesAsync();
        
        return metric;
    }

    private async Task<DashboardMetric> CalculateMetricsAsync(int? roleId = null, int? customerId = null)
    {
        var metric = new DashboardMetric
        {
            RoleId = roleId,
            CustomerId = customerId
        };

        // Get customer IDs for filtering based on RoleCoverage
        List<int>? customerIds = null;
        if (roleId.HasValue)
        {
            customerIds = await _context.RoleCoverages
                .Where(rc => rc.RoleId == roleId.Value)
                .Select(rc => rc.CustomerId)
                .ToListAsync();
        }
        else if (customerId.HasValue)
        {
            customerIds = new List<int> { customerId.Value };
        }

        // Pre-sales metrics (Plan phase)
        var preSalesQuery = _context.PreSalesProposals.AsQueryable();
        if (customerIds != null && customerIds.Any())
            preSalesQuery = preSalesQuery.Where(p => customerIds.Contains(p.CustomerId));

        metric.TotalPreSalesProposals = await preSalesQuery.CountAsync();
        metric.ActivePreSalesProposals = await preSalesQuery
            .Where(p => p.Status == PreSalesStatus.Pending || p.Status == PreSalesStatus.InReview || p.Status == PreSalesStatus.Approved)
            .CountAsync();
        
        metric.PreSalesProposalsByStageIdentification = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.InitialContact)
            .CountAsync();
        metric.PreSalesProposalsByStageQualification = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.RequirementGathering)
            .CountAsync();
        metric.PreSalesProposalsByStageProposal = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.ProposalDevelopment)
            .CountAsync();
        metric.PreSalesProposalsByStageNegotiation = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.NegotiationInProgress)
            .CountAsync();
        metric.PreSalesProposalsByStageClosedWon = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.Won)
            .CountAsync();
        metric.PreSalesProposalsByStageClosedLost = await preSalesQuery
            .Where(p => p.Stage == PreSalesStage.Lost)
            .CountAsync();

        // Case metrics (Do phase)
        var casesQuery = _context.Cases.AsQueryable();
        if (customerIds != null && customerIds.Any())
            casesQuery = casesQuery.Where(c => customerIds.Contains(c.CustomerId));

        metric.TotalCases = await casesQuery.CountAsync();
        metric.OpenCases = await casesQuery.Where(c => c.Status == CaseStatus.SupportCenter).CountAsync();
        metric.InProgressCases = await casesQuery.Where(c => c.Status == CaseStatus.LogIT).CountAsync();
        metric.ClosedCases = await casesQuery.Where(c => c.Status == CaseStatus.Closed).CountAsync();
        
        metric.CriticalPriorityCases = await casesQuery.Where(c => c.Priority == CasePriority.Critical).CountAsync();
        metric.HighPriorityCases = await casesQuery.Where(c => c.Priority == CasePriority.High).CountAsync();
        metric.MediumPriorityCases = await casesQuery.Where(c => c.Priority == CasePriority.Medium).CountAsync();
        metric.LowPriorityCases = await casesQuery.Where(c => c.Priority == CasePriority.Low).CountAsync();

        // Case resolution metrics (Check phase)
        var resolvedCases = await casesQuery
            .Where(c => c.ResolvedAt != null)
            .ToListAsync();

        if (resolvedCases.Any())
        {
            metric.CaseResolutionRate = metric.TotalCases > 0 
                ? (decimal)resolvedCases.Count / metric.TotalCases * 100 
                : 0;

            var casesWithSla = resolvedCases.Where(c => c.SlaDeadline != null).ToList();
            if (casesWithSla.Any())
            {
                metric.CasesResolvedWithinSla = casesWithSla.Count(c => c.ResolvedAt <= c.SlaDeadline);
                metric.CasesResolvedOutsideSla = casesWithSla.Count - metric.CasesResolvedWithinSla;
                metric.SlaComplianceRate = (decimal)metric.CasesResolvedWithinSla / casesWithSla.Count * 100;
            }

            var resolutionTimes = resolvedCases
                .Where(c => c.ResolvedAt.HasValue)
                .Select(c => (c.ResolvedAt!.Value - c.CreatedAt).TotalHours)
                .ToList();

            if (resolutionTimes.Any())
            {
                metric.AverageResolutionTimeHours = (decimal)resolutionTimes.Average();
            }
        }

        // Project metrics (Act phase)
        var projectsQuery = _context.Projects.AsQueryable();
        if (customerIds != null && customerIds.Any())
            projectsQuery = projectsQuery.Where(p => customerIds.Contains(p.CustomerId));

        metric.TotalProjects = await projectsQuery.CountAsync();
        metric.ActiveProjects = await projectsQuery.Where(p => p.Status == ProjectStatus.Wip).CountAsync();
        metric.CompletedProjects = await projectsQuery.Where(p => p.Status == ProjectStatus.Closed).CountAsync();
        metric.OnHoldProjects = await projectsQuery.Where(p => p.Status == ProjectStatus.Pending).CountAsync();
        
        metric.ProjectCompletionRate = metric.TotalProjects > 0 
            ? (decimal)metric.CompletedProjects / metric.TotalProjects * 100 
            : 0;

        return metric;
    }

    public async Task<PersonalDashboardDto> GetPersonalDashboardAsync(int userId, int[]? allowedCustomerIds)
    {
        var now = DateTime.UtcNow;

        // allowedCustomerIds comes pre-resolved from GetAllowedCustomerIdsAsync:
        // - null  = user has at least one unrestricted role → show all customers
        // - int[] = union of all roles' coverage → restrict to those customer IDs only
        List<int>? coverageCustomerIds = allowedCustomerIds?.ToList();

        // === Cases assigned to this user (personal ownership — no coverage filter) ===
        var openStatuses = new[]
        {
            CaseStatus.SupportCenter, CaseStatus.SC_high, CaseStatus.SC_medium, CaseStatus.SC_low
        };
        var inProgressStatuses = new[]
        {
            CaseStatus.LogIT, CaseStatus.LogIT_high, CaseStatus.LogIT_medium, CaseStatus.LogIT_low,
            CaseStatus.Customer_handling, CaseStatus.Manufacturer,
            CaseStatus.Waiting_for_work, CaseStatus.In_observation_period
        };

        var allMyCases = _context.Cases
            .Where(c => c.AssignedToUserId == userId)
            .Where(c => coverageCustomerIds == null || coverageCustomerIds.Contains(c.CustomerId));

        // Top 10 active cases ordered by latest updated/created date
        var caseItems = await allMyCases
            .Where(c => c.Status != CaseStatus.Closed)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .ThenByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new DashboardCaseItem(
                c.Id,
                c.Title,
                _context.Customers.Where(cu => cu.Id == c.CustomerId).Select(cu => cu.Name).FirstOrDefault(),
                c.Priority,
                c.Status,
                c.DueDate,
                c.SlaDeadline,
                c.SlaDeadline != null && now > c.SlaDeadline && c.ResolvedAt == null,
                _context.CaseActivities
                    .Where(a => a.CaseId == c.Id && a.ActiveFlg)
                    .Max(a => (DateTime?)a.ActivityDate),
                _context.CaseActivities
                    .Where(a => a.CaseId == c.Id && a.ActiveFlg)
                    .OrderByDescending(a => a.ActivityDate)
                    .Select(a => a.NextAction)
                    .FirstOrDefault()
            ))
            .ToListAsync();

        var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var caseStats = new DashboardCaseStats(
            MyOpenCases: await allMyCases.CountAsync(c => openStatuses.Contains(c.Status)),
            MyInProgressCases: await allMyCases.CountAsync(c => inProgressStatuses.Contains(c.Status)),
            MyResolvedThisMonth: await allMyCases.CountAsync(c => c.ResolvedAt >= firstOfMonth),
            MySlaBreachedCases: await allMyCases.CountAsync(c =>
                c.SlaDeadline != null && c.SlaDeadline < now && c.ResolvedAt == null && c.Status != CaseStatus.Closed)
        );

        // === Projects within role coverage ===
        var projectsQuery = _context.Projects.AsQueryable();
        if (coverageCustomerIds != null)
            projectsQuery = projectsQuery.Where(p => coverageCustomerIds.Contains(p.CustomerId));

        var projectItems = await projectsQuery
            .Where(p => p.Status != ProjectStatus.Closed)
            .OrderBy(p => p.Status)
            .ThenByDescending(p => p.CreatedAt)
            .Take(10)
            .Select(p => new DashboardProjectItem(
                p.Id,
                p.Name,
                _context.Customers.Where(cu => cu.Id == p.CustomerId).Select(cu => cu.Name).FirstOrDefault(),
                p.Status,
                p.CreatedAt
            ))
            .ToListAsync();

        var projectStats = new DashboardProjectStats(
            ActiveProjects: await projectsQuery.CountAsync(p => p.Status == ProjectStatus.Wip),
            CompletedProjects: await projectsQuery.CountAsync(p => p.Status == ProjectStatus.Closed),
            OnHoldProjects: await projectsQuery.CountAsync(p => p.Status == ProjectStatus.Pending)
        );

        // === Pre-sales proposals within role coverage ===
        var preSalesQuery = _context.PreSalesProposals.AsQueryable();
        if (coverageCustomerIds != null)
            preSalesQuery = preSalesQuery.Where(p => coverageCustomerIds.Contains(p.CustomerId));

        var activePipelineStatuses = new[] { PreSalesStatus.Draft, PreSalesStatus.InReview, PreSalesStatus.Pending, PreSalesStatus.Approved };
        var closedStages = new[] { PreSalesStage.Won, PreSalesStage.Lost };

        var preSalesItems = await preSalesQuery
            .Where(p => activePipelineStatuses.Contains(p.Status) && !closedStages.Contains(p.Stage))
            .OrderBy(p => p.ExpectedCloseDate == null ? DateTime.MaxValue : p.ExpectedCloseDate)
            .ThenByDescending(p => p.Stage)
            .Take(10)
            .Select(p => new DashboardPreSalesItem(
                p.Id,
                p.Title,
                _context.Customers.Where(cu => cu.Id == p.CustomerId).Select(cu => cu.Name).FirstOrDefault(),
                p.Status,
                p.Stage,
                p.ProbabilityPercentage,
                p.ExpectedCloseDate
            ))
            .ToListAsync();

        var preSalesStats = new DashboardPreSalesStats(
            InPipeline: await preSalesQuery.CountAsync(p =>
                activePipelineStatuses.Contains(p.Status) && !closedStages.Contains(p.Stage)),
            Won: await preSalesQuery.CountAsync(p => p.Stage == PreSalesStage.Won),
            LostOrRejected: await preSalesQuery.CountAsync(p =>
                p.Stage == PreSalesStage.Lost || p.Status == PreSalesStatus.Rejected)
        );

        return new PersonalDashboardDto(caseItems, caseStats, projectItems, projectStats,
            preSalesItems, preSalesStats);
    }
}

