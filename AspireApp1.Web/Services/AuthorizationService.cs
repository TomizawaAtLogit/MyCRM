using AspireApp1.Web;

namespace AspireApp1.Web.Services;

public class AuthorizationService
{
    private readonly AdminApiClient _adminApi;
    private UserDto? _currentUser;
    private HashSet<string> _userPagePermissions = new();
    private DateTime? _lastRefresh;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public AuthorizationService(AdminApiClient adminApi)
    {
        _adminApi = adminApi;
    }

    /// <summary>
    /// Check if the current user has permission to access a specific page
    /// </summary>
    /// <param name="pageName">Name of the page (e.g., "Projects", "Cases", "Admin")</param>
    /// <returns>True if the user has permission, false otherwise</returns>
    public async Task<bool> HasPageAccessAsync(string pageName)
    {
        await EnsureUserLoadedAsync();
        
        // If no user is loaded or no permissions, deny access
        if (_currentUser == null)
        {
            // For local development without authentication, allow all access
            return true;
        }

        return _userPagePermissions.Contains(pageName);
    }

    /// <summary>
    /// Get all pages the current user has access to
    /// </summary>
    public async Task<IReadOnlySet<string>> GetUserPagePermissionsAsync()
    {
        await EnsureUserLoadedAsync();
        return _userPagePermissions;
    }

    /// <summary>
    /// Refresh the user's permissions from the server
    /// </summary>
    public async Task RefreshUserPermissionsAsync()
    {
        _lastRefresh = null;
        await EnsureUserLoadedAsync();
    }

    /// <summary>
    /// Get the current user information
    /// </summary>
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        await EnsureUserLoadedAsync();
        return _currentUser;
    }

    private async Task EnsureUserLoadedAsync()
    {
        // Check if we need to refresh the cache
        if (_lastRefresh.HasValue && DateTime.UtcNow - _lastRefresh.Value < _cacheExpiration)
        {
            return;
        }

        try
        {
            // Get current Windows username
            var username = Environment.UserName;
            
            // Try to get user from API
            _currentUser = await _adminApi.GetUserByUsernameAsync(username);
            
            if (_currentUser != null && _currentUser.Roles != null)
            {
                // Collect all page permissions from all roles
                _userPagePermissions.Clear();
                foreach (var role in _currentUser.Roles)
                {
                    if (!string.IsNullOrEmpty(role.PagePermissions))
                    {
                        var permissions = role.PagePermissions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim());
                        foreach (var permission in permissions)
                        {
                            _userPagePermissions.Add(permission);
                        }
                    }
                }
            }

            _lastRefresh = DateTime.UtcNow;
        }
        catch (Exception)
        {
            // If we can't load the user, they have no permissions
            _currentUser = null;
            _userPagePermissions.Clear();
            _lastRefresh = DateTime.UtcNow; // Still cache the failure to avoid hammering the API
        }
    }
}
