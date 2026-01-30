using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories
{
    public interface IProjectTaskRepository
    {
        Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId);
        Task<ProjectTask?> GetAsync(int id);
        Task<ProjectTask> AddAsync(ProjectTask task);
        Task UpdateAsync(ProjectTask task);
        Task DeleteAsync(int id);
    }
}

