using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role> AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(int id);
}
