using Ligot.DbApi.Models;
using Ligot.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ProjectDbContext _db;

    public UserRepository(ProjectDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .ToListAsync();
    }

    public async Task<User?> GetAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByWindowsUsernameAsync(string windowsUsername)
    {
        if (string.IsNullOrWhiteSpace(windowsUsername))
        {
            return null;
        }
        
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.WindowsUsername == windowsUsername);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        // Alias method for compatibility - delegates to GetByWindowsUsernameAsync
        return await GetByWindowsUsernameAsync(username);
    }

    public async Task<User?> GetWithRolesAsync(int id)
    {
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetWithRolesByUsernameAsync(string windowsUsername)
    {
        if (string.IsNullOrWhiteSpace(windowsUsername))
        {
            return null;
        }
        
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.WindowsUsername == windowsUsername);
    }

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> AssignRoleAsync(int userId, int roleId)
    {
        // Check if already exists
        var exists = await _db.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        
        if (exists)
            return false;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        _db.UserRoles.Add(userRole);
        
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
        {
            // Duplicate key error - role already assigned (race condition)
            // Detach the entity that couldn't be saved
            _db.Entry(userRole).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId)
    {
        var userRole = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        
        if (userRole == null)
            return false;

        _db.UserRoles.Remove(userRole);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int[]?> GetAllowedCustomerIdsAsync(int userId)
    {
        // Get user with their roles and role coverages
        var user = await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RoleCoverages)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || !user.UserRoles.Any())
        {
            // User not found or has no roles - no access
            return Array.Empty<int>();
        }

        // Check if any role has no coverage (meaning access to all customers)
        var hasUnrestrictedRole = user.UserRoles
            .Any(ur => ur.Role != null && !ur.Role.RoleCoverages.Any());

        if (hasUnrestrictedRole)
        {
            // User has at least one role with no coverage restrictions - access to all customers
            return null;
        }

        // Get union of all customer IDs from all roles' coverage
        var allowedCustomerIds = user.UserRoles
            .Where(ur => ur.Role != null)
            .SelectMany(ur => ur.Role!.RoleCoverages)
            .Select(rc => rc.CustomerId)
            .Distinct()
            .ToArray();

        return allowedCustomerIds;
    }
}

