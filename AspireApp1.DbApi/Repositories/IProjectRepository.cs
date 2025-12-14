using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetAsync(int id);
        Task<Project> AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
    }
}
