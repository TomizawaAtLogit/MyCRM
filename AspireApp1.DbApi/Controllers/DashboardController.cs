using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : AuditableControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IRoleRepository _roleRepo;

    public DashboardController(
        IUserRepository userRepo, 
        IDashboardService dashboardService,
        IRoleRepository roleRepo)
        : base(userRepo)
    {
        _dashboardService = dashboardService;
        _roleRepo = roleRepo;
    }

    /// <summary>
    /// Get current dashboard metrics for the current user based on their RoleCoverage
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<DashboardMetricDto>> GetCurrentMetrics()
    {
        var (username, userId) = await GetCurrentUserInfoAsync();
        if (!userId.HasValue)
            return Ok(new DashboardMetricDto()); // Empty metrics for users without valid ID

        var user = await _userRepo.GetWithRolesAsync(userId.Value);
        if (user == null)
            return Ok(new DashboardMetricDto()); // Empty metrics

        // Get user's role with the highest priority (first role)
        var userRole = user.UserRoles.FirstOrDefault();
        if (userRole == null)
            return Ok(new DashboardMetricDto()); // Empty metrics for users without roles

        var metric = await _dashboardService.GetCurrentMetricsAsync(userRole.RoleId);
        return Ok(MapToDto(metric));
    }

    /// <summary>
    /// Get current dashboard metrics for a specific customer
    /// </summary>
    [HttpGet("current/customer/{customerId}")]
    public async Task<ActionResult<DashboardMetricDto>> GetCurrentMetricsForCustomer(int customerId)
    {
        var metric = await _dashboardService.GetCurrentMetricsAsync(customerId: customerId);
        return Ok(MapToDto(metric));
    }

    /// <summary>
    /// Get historical dashboard metrics for the current user
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<List<DashboardMetricDto>>> GetHistoricalMetrics([FromQuery] int days = 30)
    {
        var (username, userId) = await GetCurrentUserInfoAsync();
        if (!userId.HasValue)
            return Ok(new List<DashboardMetricDto>()); // Empty metrics

        var user = await _userRepo.GetWithRolesAsync(userId.Value);
        if (user == null)
            return Ok(new List<DashboardMetricDto>()); // Empty metrics

        var userRole = user.UserRoles.FirstOrDefault();
        if (userRole == null)
            return Ok(new List<DashboardMetricDto>()); // Empty metrics

        var metrics = await _dashboardService.GetHistoricalMetricsAsync(userRole.RoleId, days: days);
        return Ok(metrics.Select(MapToDto).ToList());
    }

    /// <summary>
    /// Get historical dashboard metrics for a specific customer
    /// </summary>
    [HttpGet("history/customer/{customerId}")]
    public async Task<ActionResult<List<DashboardMetricDto>>> GetHistoricalMetricsForCustomer(
        int customerId, 
        [FromQuery] int days = 30)
    {
        var metrics = await _dashboardService.GetHistoricalMetricsAsync(customerId: customerId, days: days);
        return Ok(metrics.Select(MapToDto).ToList());
    }

    /// <summary>
    /// Generate a snapshot of current metrics (admin only)
    /// </summary>
    [HttpPost("snapshot")]
    public async Task<ActionResult<DashboardMetricDto>> GenerateSnapshot()
    {
        // Generate snapshot for all roles
        var roles = await _roleRepo.GetAllAsync();
        
        foreach (var role in roles)
        {
            await _dashboardService.GenerateSnapshotAsync(role.Id);
        }

        return Ok(new { message = "Snapshots generated for all roles" });
    }

    private static DashboardMetricDto MapToDto(DashboardMetric metric)
    {
        return new DashboardMetricDto
        {
            SnapshotDate = metric.SnapshotDate,
            RoleId = metric.RoleId,
            CustomerId = metric.CustomerId,
            TotalPreSalesProposals = metric.TotalPreSalesProposals,
            ActivePreSalesProposals = metric.ActivePreSalesProposals,
            PreSalesProposalsByStageIdentification = metric.PreSalesProposalsByStageIdentification,
            PreSalesProposalsByStageQualification = metric.PreSalesProposalsByStageQualification,
            PreSalesProposalsByStageProposal = metric.PreSalesProposalsByStageProposal,
            PreSalesProposalsByStageNegotiation = metric.PreSalesProposalsByStageNegotiation,
            PreSalesProposalsByStageClosedWon = metric.PreSalesProposalsByStageClosedWon,
            PreSalesProposalsByStageClosedLost = metric.PreSalesProposalsByStageClosedLost,
            TotalCases = metric.TotalCases,
            OpenCases = metric.OpenCases,
            InProgressCases = metric.InProgressCases,
            ResolvedCases = metric.ResolvedCases,
            ClosedCases = metric.ClosedCases,
            CriticalPriorityCases = metric.CriticalPriorityCases,
            HighPriorityCases = metric.HighPriorityCases,
            MediumPriorityCases = metric.MediumPriorityCases,
            LowPriorityCases = metric.LowPriorityCases,
            CaseResolutionRate = metric.CaseResolutionRate,
            SlaComplianceRate = metric.SlaComplianceRate,
            AverageResolutionTimeHours = metric.AverageResolutionTimeHours,
            CasesResolvedWithinSla = metric.CasesResolvedWithinSla,
            CasesResolvedOutsideSla = metric.CasesResolvedOutsideSla,
            TotalProjects = metric.TotalProjects,
            ActiveProjects = metric.ActiveProjects,
            CompletedProjects = metric.CompletedProjects,
            OnHoldProjects = metric.OnHoldProjects,
            ProjectCompletionRate = metric.ProjectCompletionRate
        };
    }
}
