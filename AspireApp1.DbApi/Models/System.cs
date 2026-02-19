namespace AspireApp1.DbApi.Models;

public class System
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<SystemComponent> Components { get; set; } = new List<SystemComponent>();
}
