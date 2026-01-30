namespace Ligot.DbApi.Models;

public class CustomerSite
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Customer Customer { get; set; } = null!;
}

