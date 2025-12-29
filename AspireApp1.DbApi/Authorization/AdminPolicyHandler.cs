using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AspireApp1.DbApi.Authorization;

public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public AdminPolicyHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AdminPolicyRequirement requirement)
    {
        var username = context.User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return;
        }

        // Create a scope to resolve scoped services
        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = await userRepo.GetWithRolesByUsernameAsync(username);
        if (user == null || !user.IsActive)
        {
            return;
        }

        // Check if user has a role with "Admin" in page permissions
        var hasAdminPermission = user.UserRoles
            .Any(ur => ur.Role.PagePermissions.Contains("Admin", StringComparison.OrdinalIgnoreCase));

        if (hasAdminPermission)
        {
            context.Succeed(requirement);
        }
    }
}

public class AdminPolicyRequirement : IAuthorizationRequirement
{
}
