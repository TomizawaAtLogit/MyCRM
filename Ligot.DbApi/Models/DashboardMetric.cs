namespace Ligot.DbApi.Models;

public class DashboardMetric
{
    public int Id { get; set; }
    public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;
    public int? RoleId { get; set; }
    public int? CustomerId { get; set; }
    
    // Pre-sales metrics (Plan phase)
    public int TotalPreSalesProposals { get; set; }
    public int ActivePreSalesProposals { get; set; }
    public int PreSalesProposalsByStageIdentification { get; set; }
    public int PreSalesProposalsByStageQualification { get; set; }
    public int PreSalesProposalsByStageProposal { get; set; }
    public int PreSalesProposalsByStageNegotiation { get; set; }
    public int PreSalesProposalsByStageClosedWon { get; set; }
    public int PreSalesProposalsByStageClosedLost { get; set; }
    
    // Case metrics (Do phase)
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int InProgressCases { get; set; }
    public int ResolvedCases { get; set; }
    public int ClosedCases { get; set; }
    public int CriticalPriorityCases { get; set; }
    public int HighPriorityCases { get; set; }
    public int MediumPriorityCases { get; set; }
    public int LowPriorityCases { get; set; }
    
    // Case resolution metrics (Check phase)
    public decimal CaseResolutionRate { get; set; }
    public decimal SlaComplianceRate { get; set; }
    public decimal AverageResolutionTimeHours { get; set; }
    public int CasesResolvedWithinSla { get; set; }
    public int CasesResolvedOutsideSla { get; set; }
    
    // Project metrics (Act phase)
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OnHoldProjects { get; set; }
    public decimal ProjectCompletionRate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Role? Role { get; set; }
    public Customer? Customer { get; set; }
}

