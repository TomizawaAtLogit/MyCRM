namespace Ligot.DbApi.Models;

public class RoleCoverage
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int CustomerId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}

