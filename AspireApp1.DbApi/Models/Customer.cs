namespace AspireApp1.DbApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CustomerDatabase> Databases { get; set; } = new List<CustomerDatabase>();
    public ICollection<CustomerSite> Sites { get; set; } = new List<CustomerSite>();
    public ICollection<CustomerSystem> Systems { get; set; } = new List<CustomerSystem>();
    public ICollection<System> NewSystems { get; set; } = new List<System>();
    public ICollection<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
    public ICollection<ProjectActivity> ProjectActivities { get; set; } = new List<ProjectActivity>();
}
