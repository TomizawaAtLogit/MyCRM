namespace AspireApp1.DbApi.Models;

public class CustomerSystem
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string? ComponentType { get; set; } // e.g., Server, Storage, Network
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Location { get; set; }
    public DateTime? InstallationDate { get; set; }
    public DateTime? WarrantyExpiration { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Customer Customer { get; set; } = null!;
}
