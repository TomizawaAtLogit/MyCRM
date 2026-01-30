using Ligot.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Ligot.DbApi.Authorization;

public class SupportPolicyHandler : AuthorizationHandler<SupportPolicyRequirement>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SupportPolicyHandler> _logger;

    public SupportPolicyHandler(IServiceProvider serviceProvider, ILogger<SupportPolicyHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        SupportPolicyRequirement requirement)
    {
        var username = context.User.Identity?.Name;
        _logger.LogInformation("SupportPolicyHandler: Username from context: {Username}", username);
        
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
            _logger.LogInformation("SupportPolicyHandler: Using environment username: {Username}", username);
        }

        var usernameOnly = username.Contains('\\') 
            ? username.Split('\\')[1] 
            : username;
        
        _logger.LogInformation("SupportPolicyHandler: Looking up user: {UsernameOnly}", usernameOnly);

        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = await userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        if (user == null)
        {
            _logger.LogWarning("SupportPolicyHandler: User not found in database: {UsernameOnly}", usernameOnly);
            return;
        }
        
        if (!user.IsActive)
        {
            _logger.LogWarning("SupportPolicyHandler: User is not active: {UsernameOnly}", usernameOnly);
            return;
        }

        _logger.LogInformation("SupportPolicyHandler: User found with {RoleCount} roles", user.UserRoles?.Count ?? 0);

        var hasSupportPermission = user.UserRoles
            ?.Any(ur => ur.Role.PagePermissions.Contains("Support", StringComparison.OrdinalIgnoreCase) ||
                        ur.Role.PagePermissions.Contains("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;

        _logger.LogInformation("SupportPolicyHandler: Has support permission: {HasPermission}", hasSupportPermission);

        if (hasSupportPermission)
        {
            context.Succeed(requirement);
        }
    }
}

public class SupportPolicyRequirement : IAuthorizationRequirement
{
}

