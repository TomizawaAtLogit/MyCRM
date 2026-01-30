using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Services;

public interface IDashboardService
{
    Task<DashboardMetric> GetCurrentMetricsAsync(int? roleId = null, int? customerId = null);
    Task<List<DashboardMetric>> GetHistoricalMetricsAsync(int? roleId = null, int? customerId = null, int days = 30);
    Task<DashboardMetric> GenerateSnapshotAsync(int? roleId = null, int? customerId = null);
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
        metric.OpenCases = await casesQuery.Where(c => c.Status == CaseStatus.Open).CountAsync();
        metric.InProgressCases = await casesQuery.Where(c => c.Status == CaseStatus.InProgress).CountAsync();
        metric.ResolvedCases = await casesQuery.Where(c => c.Status == CaseStatus.Resolved).CountAsync();
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
}

