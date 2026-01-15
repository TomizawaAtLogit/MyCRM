using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface IRequirementDefinitionRepository
    {
        Task<IEnumerable<RequirementDefinition>> GetAllAsync();
        Task<IEnumerable<RequirementDefinition>> GetByCustomerIdAsync(int customerId);
        Task<RequirementDefinition?> GetAsync(int id);
        Task<RequirementDefinition> AddAsync(RequirementDefinition entity);
        Task UpdateAsync(RequirementDefinition entity);
        Task DeleteAsync(int id);
    }
}
