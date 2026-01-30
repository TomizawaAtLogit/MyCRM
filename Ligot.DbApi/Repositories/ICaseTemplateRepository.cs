using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories;

public interface ICaseTemplateRepository
{
    Task<IEnumerable<CaseTemplate>> GetAllActiveAsync();
    Task<IEnumerable<CaseTemplate>> GetAllAsync();
    Task<CaseTemplate?> GetAsync(int id);
    Task<CaseTemplate> AddAsync(CaseTemplate template);
    Task UpdateAsync(CaseTemplate template);
    Task DeleteAsync(int id);
}

