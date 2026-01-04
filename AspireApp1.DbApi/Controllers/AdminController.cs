using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Policy = "AdminOnly")] // DISABLED FOR LOCAL DEVELOPMENT
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public AdminController(IUserRepository userRepo, IRoleRepository roleRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
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
        return user;
    }

    [HttpGet("users/by-username/{username}")]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        var user = await _userRepo.GetWithRolesByUsernameAsync(username);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost("users")]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        var created = await _userRepo.AddAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
            return BadRequest();
        await _userRepo.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userRepo.DeleteAsync(id);
        return NoContent();
    }

    // User role assignments
    [HttpPost("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var success = await _userRepo.AssignRoleAsync(userId, roleId);
        if (!success)
            return Conflict("Role already assigned to user");
        return NoContent();
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var success = await _userRepo.RemoveRoleAsync(userId, roleId);
        if (!success)
            return NotFound();
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
            UserCount = r.UserRoles?.Count ?? 0
        });
    }

    [HttpGet("roles/{id}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await _roleRepo.GetAsync(id);
        if (role == null)
            return NotFound();
        return role;
    }

    [HttpPost("roles")]
    public async Task<ActionResult<Role>> PostRole(Role role)
    {
        var created = await _roleRepo.AddAsync(role);
        return CreatedAtAction(nameof(GetRole), new { id = created.Id }, created);
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> PutRole(int id, Role role)
    {
        if (id != role.Id)
            return BadRequest();
        await _roleRepo.UpdateAsync(role);
        return NoContent();
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await _roleRepo.DeleteAsync(id);
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
}
