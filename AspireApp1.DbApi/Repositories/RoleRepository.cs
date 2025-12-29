using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

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
}
