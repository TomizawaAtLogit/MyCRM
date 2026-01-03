namespace AspireApp1.DbApi.Models;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Read, Update, Delete
    public string EntityType { get; set; } = string.Empty; // Customer, Order, Project, etc.
    public int? EntityId { get; set; }
    public string? EntitySnapshot { get; set; } // JSON snapshot of the entity at the time of action
    public DateTime RetentionUntil { get; set; } // Calculated as Timestamp + retention period
    
    // Navigation property
    public User? User { get; set; }
}
