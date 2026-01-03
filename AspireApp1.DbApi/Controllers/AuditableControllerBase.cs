using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

public abstract class AuditableControllerBase : ControllerBase
{
    private readonly IUserRepository _userRepo;

    protected AuditableControllerBase(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    protected async Task<(string username, int? userId)> GetCurrentUserInfoAsync()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var usernameOnly = username.Contains('\\') ? username.Split('\\')[1] : username;
        
        var user = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        return (usernameOnly, user?.Id);
    }
}
