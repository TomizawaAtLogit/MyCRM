using Ligot.DbApi.Models;
using Ligot.DbApi.Repositories;
using Ligot.DbApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ligot.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Policy = "AdminOnly")] // DISABLED FOR LOCAL DEVELOPMENT
public class AdminController : AuditableControllerBase
{
    private readonly IRoleRepository _roleRepo;
    private readonly IAuditService _auditService;

    public AdminController(IUserRepository userRepo, IRoleRepository roleRepo, IAuditService auditService)
        : base(userRepo)
    {
        _roleRepo = roleRepo;
        _auditService = auditService;
    }

    // User management
    [HttpGet("users")]
    public async Task<IEnumerable<object>> GetUsers()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => new
        {
            u.Id,
            u.WindowsUsername,
            u.DisplayName,
            u.Email,
            u.IsActive,
            Roles = u.UserRoles.Select(ur => new
            {
                ur.Role.Id,
                ur.Role.Name,
                ur.Role.Description,
                ur.Role.PagePermissions
            }).ToList()
        });
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepo.GetWithRolesAsync(id);
        if (user == null)
            return NotFound();
        
        // Log read action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "User", id, user);
        
        return user;
    }

    [HttpGet("users/by-username/{username}")]
    public async Task<ActionResult<object>> GetUserByUsername(string username)
    {
        var user = await _userRepo.GetWithRolesByUsernameAsync(username);
        if (user == null)
            return NotFound();
        
        return new
        {
            user.Id,
            user.WindowsUsername,
            user.DisplayName,
            user.Email,
            user.IsActive,
            Roles = user.UserRoles.Select(ur => new
            {
                ur.Role.Id,
                ur.Role.Name,
                ur.Role.Description,
                ur.Role.PagePermissions
            }).ToList()
        };
    }

    [HttpPost("users")]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        var created = await _userRepo.AddAsync(user);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "User", created.Id, created);
        
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
            return BadRequest();
        await _userRepo.UpdateAsync(user);
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "User", id, user);
        
        return NoContent();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userRepo.DeleteAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "User", id, null);
        
        return NoContent();
    }

    // User role assignments
    [HttpPost("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var success = await _userRepo.AssignRoleAsync(userId, roleId);
        if (!success)
            return Conflict("Role already assigned to user");
        
        // Log assign role action
        var (username, currentUserId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, currentUserId, "AssignRole", "UserRole", 0, new { UserId = userId, RoleId = roleId });
        
        return NoContent();
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var success = await _userRepo.RemoveRoleAsync(userId, roleId);
        if (!success)
            return NotFound();
        
        // Log remove role action
        var (username, currentUserId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, currentUserId, "RemoveRole", "UserRole", 0, new { UserId = userId, RoleId = roleId });
        
        return NoContent();
    }

    // Role management
    [HttpGet("roles")]
    public async Task<IEnumerable<object>> GetRoles()
    {
        var roles = await _roleRepo.GetAllWithUserCountAsync();
        return roles.Select(r => new
        {
            r.Id,
            r.Name,
            r.Description,
            r.PagePermissions,
            UserCount = r.UserRoles?.Count ?? 0,
            CoverageCount = r.RoleCoverages?.Count ?? 0
        });
    }

    [HttpGet("roles/{id}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await _roleRepo.GetAsync(id);
        if (role == null)
            return NotFound();
        
        // Log read action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "Role", id, role);
        
        return role;
    }

    [HttpPost("roles")]
    public async Task<ActionResult<Role>> PostRole(Role role)
    {
        var created = await _roleRepo.AddAsync(role);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "Role", created.Id, created);
        
        return CreatedAtAction(nameof(GetRole), new { id = created.Id }, created);
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> PutRole(int id, Role role)
    {
        if (id != role.Id)
            return BadRequest();
        await _roleRepo.UpdateAsync(role);
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "Role", id, role);
        
        return NoContent();
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await _roleRepo.DeleteAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "Role", id, null);
        
        return NoContent();
    }

    // Get users by role
    [HttpGet("roles/{id}/users")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsersByRole(int id)
    {
        var users = await _roleRepo.GetUsersByRoleAsync(id);
        return Ok(users.Select(u => new
        {
            u.Id,
            u.WindowsUsername,
            u.DisplayName,
            u.Email,
            u.IsActive
        }));
    }

    // Coverage management
    [HttpGet("roles/{id}/coverage")]
    public async Task<ActionResult<IEnumerable<object>>> GetCustomersByRole(int id)
    {
        var customers = await _roleRepo.GetCustomersByRoleAsync(id);
        return Ok(customers.Select(c => new
        {
            c.Id,
            c.Name,
            c.ContactPerson,
            c.Email,
            c.Phone
        }));
    }

    [HttpPost("roles/{roleId}/coverage/{customerId}")]
    public async Task<IActionResult> AssignCoverage(int roleId, int customerId)
    {
        var success = await _roleRepo.AssignCoverageAsync(roleId, customerId);
        if (!success)
            return Conflict("Coverage already assigned to role");
        
        // Log assign coverage action
        var (username, currentUserId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, currentUserId, "AssignCoverage", "RoleCoverage", 0, new { RoleId = roleId, CustomerId = customerId });
        
        return NoContent();
    }

    [HttpDelete("roles/{roleId}/coverage/{customerId}")]
    public async Task<IActionResult> RemoveCoverage(int roleId, int customerId)
    {
        var success = await _roleRepo.RemoveCoverageAsync(roleId, customerId);
        if (!success)
            return NotFound();
        
        // Log remove coverage action
        var (username, currentUserId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, currentUserId, "RemoveCoverage", "RoleCoverage", 0, new { RoleId = roleId, CustomerId = customerId });
        
        return NoContent();
    }
}

