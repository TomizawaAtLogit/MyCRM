namespace Ligot.DbApi.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., Admin, Manager, User
    public string? Description { get; set; }
    public string PagePermissions { get; set; } = string.Empty; // Comma-separated: "Projects,Customers,Admin"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleCoverage> RoleCoverages { get; set; } = new List<RoleCoverage>();
}

