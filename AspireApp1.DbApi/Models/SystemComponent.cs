namespace AspireApp1.DbApi.Models;

public class SystemComponent
{
    public int Id { get; set; }
    public int SystemId { get; set; }
    public string ComponentType { get; set; } = string.Empty; // e.g., Server, Storage, Network
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? WarrantyExpiration { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public System System { get; set; } = null!;
}
