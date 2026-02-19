namespace AspireApp1.DbApi.Models;

public class PreSalesProposal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CustomerId { get; set; }
    public int? RequirementDefinitionId { get; set; }
    public int? CustomerOrderId { get; set; }
    public PreSalesStatus Status { get; set; } = PreSalesStatus.Draft;
    public PreSalesStage Stage { get; set; } = PreSalesStage.InitialContact;
    public int? AssignedToUserId { get; set; }
    public decimal? EstimatedValue { get; set; }
    public int? ProbabilityPercentage { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public RequirementDefinition? RequirementDefinition { get; set; }
    public CustomerOrder? CustomerOrder { get; set; }
    public User? AssignedToUser { get; set; }
}
