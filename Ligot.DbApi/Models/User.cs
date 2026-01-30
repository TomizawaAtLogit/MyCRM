namespace Ligot.DbApi.Models;

public class User
{
    public int Id { get; set; }
    public string WindowsUsername { get; set; } = string.Empty; // DOMAIN\username
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PreferredLanguage { get; set; } // Language code (e.g., "en", "ja")
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

