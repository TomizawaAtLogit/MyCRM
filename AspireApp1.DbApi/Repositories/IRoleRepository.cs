using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetAllWithUserCountAsync();
    Task<Role?> GetAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role> AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(int id);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    
    // Coverage methods
    Task<IEnumerable<Customer>> GetCustomersByRoleAsync(int roleId);
    Task<bool> AssignCoverageAsync(int roleId, int customerId);
    Task<bool> RemoveCoverageAsync(int roleId, int customerId);
}
