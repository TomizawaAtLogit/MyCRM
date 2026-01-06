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
        var username = User.Identity?.Name;
        
        // If authentication is disabled (username is null), use environment username for local dev
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
        }
        
        var usernameOnly = ExtractUsernameWithoutDomain(username);
        
        // Only attempt to look up the user if we have a valid username
        if (string.IsNullOrEmpty(usernameOnly))
        {
            return ("Unknown", null);
        }
        
        var user = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        return (usernameOnly, user?.Id);
    }

    protected static string ExtractUsernameWithoutDomain(string username)
    {
        return username.Contains('\\') ? username.Split('\\')[1] : username;
    }
}
