using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetAsync(int id);
    Task<User?> GetByWindowsUsernameAsync(string windowsUsername);
    Task<User?> GetWithRolesAsync(int id);
    Task<User?> GetWithRolesByUsernameAsync(string windowsUsername);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> AssignRoleAsync(int userId, int roleId);
    Task<bool> RemoveRoleAsync(int userId, int roleId);
}
