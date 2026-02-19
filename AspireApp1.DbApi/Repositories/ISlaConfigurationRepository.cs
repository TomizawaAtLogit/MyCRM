using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface ISlaConfigurationRepository
{
    Task<IEnumerable<SlaThreshold>> GetAllAsync();
    Task<SlaThreshold?> GetByPriorityAsync(CasePriority priority);
    Task<SlaThreshold?> GetAsync(int id);
    Task<SlaThreshold> AddAsync(SlaThreshold config);
    Task UpdateAsync(SlaThreshold config);
    Task DeleteAsync(int id);
}
