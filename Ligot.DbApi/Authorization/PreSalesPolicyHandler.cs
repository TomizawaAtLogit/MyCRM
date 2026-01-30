using Ligot.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Ligot.DbApi.Authorization;

public class PreSalesPolicyHandler : AuthorizationHandler<PreSalesPolicyRequirement>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PreSalesPolicyHandler> _logger;

    public PreSalesPolicyHandler(IServiceProvider serviceProvider, ILogger<PreSalesPolicyHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PreSalesPolicyRequirement requirement)
    {
        var username = context.User.Identity?.Name;
        _logger.LogInformation("PreSalesPolicyHandler: Username from context: {Username}", username);
        
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
            _logger.LogInformation("PreSalesPolicyHandler: Using environment username: {Username}", username);
        }

        var usernameOnly = username.Contains('\\') 
            ? username.Split('\\')[1] 
            : username;
        
        _logger.LogInformation("PreSalesPolicyHandler: Looking up user: {UsernameOnly}", usernameOnly);

        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = await userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        if (user == null)
        {
            _logger.LogWarning("PreSalesPolicyHandler: User not found in database: {UsernameOnly}", usernameOnly);
            return;
        }
        
        if (!user.IsActive)
        {
            _logger.LogWarning("PreSalesPolicyHandler: User is not active: {UsernameOnly}", usernameOnly);
            return;
        }

        _logger.LogInformation("PreSalesPolicyHandler: User found with {RoleCount} roles", user.UserRoles?.Count ?? 0);

        var hasPreSalesPermission = user.UserRoles
            ?.Any(ur => PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "PreSales", new[] { "ReadOnly", "FullControl" }) ||
                        PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "Admin", new[] { "ReadOnly", "FullControl" })) ?? false;

        _logger.LogInformation("PreSalesPolicyHandler: Has pre-sales permission: {HasPermission}", hasPreSalesPermission);

        if (hasPreSalesPermission)
        {
            context.Succeed(requirement);
        }
    }
}

public class PreSalesPolicyRequirement : IAuthorizationRequirement
{
}

