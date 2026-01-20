using AspireApp1.DbApi.Authorization;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // All authenticated users can access - DISABLED FOR LOCAL DEVELOPMENT
public class AuditsController : AuditableControllerBase
{
    private readonly IAuditRepository _auditRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<AuditsController> _logger;

    public AuditsController(IAuditRepository auditRepo, IUserRepository userRepo, ILogger<AuditsController> logger)
        : base(userRepo)
    {
        _auditRepo = auditRepo;
        _userRepo = userRepo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
        [FromQuery] bool? viewAll = null,
        [FromQuery] string? entityType = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var username = User.Identity?.Name;
        
        // If authentication is disabled (username is null), use environment username for local dev
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
        }

        // Extract username without domain if present
        var usernameOnly = ExtractUsernameWithoutDomain(username);
        
        // Get current user
        var currentUser = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        if (currentUser == null)
        {
            _logger.LogWarning("User not found in database: {Username}", usernameOnly);
            return Forbid();
        }

        // Check if user is admin (new format: "Admin:FullControl" or legacy: "Admin")
        var isAdmin = currentUser.UserRoles
            ?.Any(ur => PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "Admin", new[] { "ReadOnly", "FullControl" })) ?? false;

        // Determine which logs to return
        int? filterUserId = null;
        if (!isAdmin || (viewAll != true))
        {
            // Non-admin users or admin without viewAll flag see only their own logs
            filterUserId = currentUser.Id;
        }

        var logs = await _auditRepo.GetFilteredAsync(
            userId: filterUserId,
            entityType: entityType,
            action: action,
            fromDate: fromDate,
            toDate: toDate);

        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuditLog>> GetAuditLog(int id)
    {
        var log = await _auditRepo.GetAsync(id);
        if (log == null)
        {
            return NotFound();
        }

        var username = User.Identity?.Name;
        
        // If authentication is disabled (username is null), use environment username for local dev
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
        }

        var usernameOnly = ExtractUsernameWithoutDomain(username);
        var currentUser = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        
        if (currentUser == null)
        {
            return Forbid();
        }

        // Check if user is admin (new format: "Admin:FullControl" or legacy: "Admin")
        var isAdmin = currentUser.UserRoles
            ?.Any(ur => PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "Admin", new[] { "ReadOnly", "FullControl" })) ?? false;

        // Non-admin users can only view their own logs
        if (!isAdmin && log.UserId != currentUser.Id)
        {
            return Forbid();
        }

        return Ok(log);
    }

    [HttpGet("is-admin")]
    public async Task<ActionResult<bool>> IsAdmin()
    {
        var username = User.Identity?.Name;
        
        // If authentication is disabled (username is null), use environment username for local dev
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
        }

        var usernameOnly = ExtractUsernameWithoutDomain(username);
        var currentUser = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        
        if (currentUser == null)
        {
            return Ok(false);
        }

        var isAdmin = currentUser.UserRoles
            ?.Any(ur => PagePermissionHelper.HasPagePermission(ur.Role.PagePermissions, "Admin", new[] { "ReadOnly", "FullControl" })) ?? false;

        return Ok(isAdmin);
    }
}
