namespace Ligot.DbApi.Models;

public class CustomerDatabase
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string? DatabaseType { get; set; } // e.g., PostgreSQL, SQL Server, MySQL
    public string? ServerName { get; set; }
    public string? Port { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Customer Customer { get; set; } = null!;
}

