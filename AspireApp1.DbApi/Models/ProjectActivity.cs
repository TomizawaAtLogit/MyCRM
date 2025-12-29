namespace AspireApp1.DbApi.Models;

public class ProjectActivity
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; } // Wide text area for detailed activity
    public string? NextAction { get; set; }
    public string? ActivityType { get; set; } // e.g., Meeting, Support, Development
    public string? PerformedBy { get; set; } // Username who performed the activity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public Customer? Customer { get; set; }
}
