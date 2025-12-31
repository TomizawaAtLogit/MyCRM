using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface IProjectActivityRepository
{
    Task<IEnumerable<ProjectActivity>> GetAllAsync();
    Task<ProjectActivity?> GetAsync(int id);
    Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<ProjectActivity>> SearchAsync(DateTime? startDate, DateTime? endDate, string? activityType);
    Task<ProjectActivity> AddAsync(ProjectActivity activity);
    Task UpdateAsync(ProjectActivity activity);
    Task DeleteAsync(int id);
}
