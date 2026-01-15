namespace AspireApp1.DbApi.Models;

public class PreSalesActivity
{
    public int Id { get; set; }
    public int PreSalesProposalId { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? NextAction { get; set; }
    public string? ActivityType { get; set; } // e.g., Meeting, Email, Call, Presentation
    public string? PerformedBy { get; set; }
    public int? PreviousAssignedToUserId { get; set; }
    public int? NewAssignedToUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public PreSalesProposal PreSalesProposal { get; set; } = null!;
}
