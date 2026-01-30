namespace Ligot.DbApi.Models;

/// <summary>
/// Shared table for requirement definitions that can be referenced by Pre-sales proposals
/// </summary>
public class RequirementDefinition
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CustomerId { get; set; }
    public string? Category { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
}

