namespace Ligot.DbApi.Models;

public class PreSalesWorkHour
{
    public int Id { get; set; }
    public int PreSalesProposalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int NumberOfPeople { get; set; }
    public int WorkingHours { get; set; }
    public int HourlyWage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public PreSalesProposal PreSalesProposal { get; set; } = null!;

    // Calculated property
    public int TotalCost => HourlyWage * NumberOfPeople * WorkingHours;
}

