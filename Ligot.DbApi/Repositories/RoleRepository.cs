using Ligot.DbApi.Models;
using Ligot.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ProjectDbContext _db;

    public RoleRepository(ProjectDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetAllWithUserCountAsync()
    {
        return await _db.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RoleCoverages)
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        return await _db.Users
            .Where(u => u.UserRoles!.Any(ur => ur.RoleId == roleId))
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .ToListAsync();
    }

    public async Task<Role?> GetAsync(int id)
    {
        return await _db.Roles.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Role> AddAsync(Role role)
    {
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return role;
    }

    public async Task UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;
        _db.Roles.Update(role);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role != null)
        {
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Customer>> GetCustomersByRoleAsync(int roleId)
    {
        return await _db.Customers
            .Where(c => c.RoleCoverages!.Any(rc => rc.RoleId == roleId))
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> AssignCoverageAsync(int roleId, int customerId)
    {
        // Check if coverage already exists
        var exists = await _db.RoleCoverages
            .AnyAsync(rc => rc.RoleId == roleId && rc.CustomerId == customerId);
        
        if (exists)
            return false;

        var roleCoverage = new RoleCoverage
        {
            RoleId = roleId,
            CustomerId = customerId
        };

        _db.RoleCoverages.Add(roleCoverage);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveCoverageAsync(int roleId, int customerId)
    {
        var roleCoverage = await _db.RoleCoverages
            .FirstOrDefaultAsync(rc => rc.RoleId == roleId && rc.CustomerId == customerId);
        
        if (roleCoverage == null)
            return false;

        _db.RoleCoverages.Remove(roleCoverage);
        await _db.SaveChangesAsync();
        return true;
    }
}

