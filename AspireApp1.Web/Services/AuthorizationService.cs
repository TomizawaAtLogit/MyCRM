using AspireApp1.Web;
using Microsoft.Extensions.Logging;

namespace AspireApp1.Web.Services;

public class AuthorizationService
{
    private readonly AdminApiClient _adminApi;
    private readonly ILogger<AuthorizationService> _logger;
    private UserDto? _currentUser;
    private HashSet<string> _userPagePermissions = new();
    private DateTime? _lastRefresh;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _failureCacheExpiration = TimeSpan.FromSeconds(30);
    private bool _lastLoadWasFailure = false;
    
    // For development only - should be false in production
    private readonly bool _allowUnauthenticatedAccess;

    public AuthorizationService(AdminApiClient adminApi, ILogger<AuthorizationService> logger, IConfiguration configuration, IHostEnvironment environment)
    {
        _adminApi = adminApi;
        _logger = logger;
        // In development mode without authentication, allow unauthenticated access by default
        // In production, always require authentication (default: false)
        var defaultValue = environment.IsDevelopment();
        _allowUnauthenticatedAccess = configuration.GetValue<bool>("Development:AllowUnauthenticatedAccess", defaultValue);
        
        if (_allowUnauthenticatedAccess && !environment.IsDevelopment())
        {
            _logger.LogWarning("SECURITY WARNING: Unauthenticated access is enabled in a non-development environment!");
        }
    }

    /// <summary>
    /// Check if the current user has permission to access a specific page
    /// </summary>
    /// <param name="pageName">Name of the page (e.g., "Projects", "Cases", "Admin")</param>
    /// <returns>True if the user has permission, false otherwise</returns>
    public async Task<bool> HasPageAccessAsync(string pageName)
    {
        await EnsureUserLoadedAsync();
        
        // If no user is loaded, check if we allow unauthenticated access (development only)
        if (_currentUser == null)
        {
            if (_allowUnauthenticatedAccess)
            {
                _logger.LogWarning("Allowing unauthenticated access to page '{PageName}' - development mode only", pageName);
                return true;
            }
            
            _logger.LogWarning("Denying access to page '{PageName}' - user not authenticated", pageName);
            return false;
        }

        var hasAccess = _userPagePermissions.Contains(pageName);
        if (!hasAccess)
        {
            _logger.LogInformation("User '{Username}' denied access to page '{PageName}'", _currentUser.WindowsUsername, pageName);
        }
        
        return hasAccess;
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
        // Use shorter cache duration for failures to allow quicker recovery
        var cacheExpiration = _lastLoadWasFailure ? _failureCacheExpiration : _cacheExpiration;
        if (_lastRefresh.HasValue && DateTime.UtcNow - _lastRefresh.Value < cacheExpiration)
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
            _lastLoadWasFailure = false;
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            _logger.LogError(ex, "Failed to load user permissions for username '{Username}'", Environment.UserName);
            
            // If we can't load the user, they have no permissions
            _currentUser = null;
            _userPagePermissions.Clear();
            _lastRefresh = DateTime.UtcNow;
            _lastLoadWasFailure = true; // Use shorter cache for failures
        }
    }
}
