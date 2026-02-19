using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AspireApp1.DbApi.Authorization;

public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AdminPolicyHandler> _logger;

    public AdminPolicyHandler(IServiceProvider serviceProvider, ILogger<AdminPolicyHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AdminPolicyRequirement requirement)
    {
        var username = context.User.Identity?.Name;
        _logger.LogInformation("AdminPolicyHandler: Username from context: {Username}", username);
        
        // If authentication is disabled (username is null), use environment username for local dev
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
            _logger.LogInformation("AdminPolicyHandler: Using environment username: {Username}", username);
        }

        // Try to extract just the username without domain
        var usernameOnly = username.Contains('\\') 
            ? username.Split('\\')[1] 
            : username;
        
        _logger.LogInformation("AdminPolicyHandler: Looking up user: {UsernameOnly}", usernameOnly);

        // Create a scope to resolve scoped services
        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = await userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        if (user == null)
        {
            _logger.LogWarning("AdminPolicyHandler: User not found in database: {UsernameOnly}", usernameOnly);
            return;
        }
        
        if (!user.IsActive)
        {
            _logger.LogWarning("AdminPolicyHandler: User is not active: {UsernameOnly}", usernameOnly);
            return;
        }

        _logger.LogInformation("AdminPolicyHandler: User found with {RoleCount} roles", user.UserRoles?.Count ?? 0);

        // Check if user has a role with "Admin" page permission (new format: "Admin:FullControl" or legacy: "Admin")
        var hasAdminPermission = user.UserRoles
            ?.Any(ur => PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "Admin", new[] { "ReadOnly", "FullControl" })) ?? false;

        _logger.LogInformation("AdminPolicyHandler: Has admin permission: {HasPermission}", hasAdminPermission);

        if (hasAdminPermission)
        {
            context.Succeed(requirement);
        }
    }
}

public class AdminPolicyRequirement : IAuthorizationRequirement
{
}
