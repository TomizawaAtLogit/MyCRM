namespace AspireApp1.DbApi.Authorization;

/// <summary>
/// Helper utility for checking page permissions with support for permission levels.
/// Handles both new format ("Page:Level") and legacy format ("Page").
/// </summary>
public static class PagePermissionHelper
{
    /// <summary>
    /// Check if a permission string contains a specific page with allowed permission levels.
    /// </summary>
    /// <param name="pagePermissionsString">Permission string (e.g., "Admin:FullControl,Projects:ReadOnly")</param>
    /// <param name="pageName">Page name to check for</param>
    /// <param name="allowedLevels">Allowed permission levels (e.g., "ReadOnly", "FullControl")</param>
    /// <returns>True if the page has one of the allowed permission levels</returns>
    public static bool HasPagePermission(string? pagePermissionsString, string pageName, string[] allowedLevels)
    {
        if (string.IsNullOrEmpty(pagePermissionsString))
            return false;

        var permissions = pagePermissionsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var perm in permissions)
        {
            var trimmed = perm.Trim();
            
            // Parse format "Page:Level" or legacy format "Page"
            var parts = trimmed.Split(':');
            var permPage = parts[0].Trim();
            var permLevel = parts.Length > 1 ? parts[1].Trim() : "FullControl";
            
            if (permPage.Equals(pageName, StringComparison.OrdinalIgnoreCase))
            {
                return allowedLevels.Any(l => l.Equals(permLevel, StringComparison.OrdinalIgnoreCase));
            }
        }
        
        return false;
    }
}
