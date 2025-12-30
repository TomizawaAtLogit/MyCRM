namespace AspireApp1.DbApi.Models;

public class System
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? InstallationDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<SystemComponent> Components { get; set; } = new List<SystemComponent>();
}
