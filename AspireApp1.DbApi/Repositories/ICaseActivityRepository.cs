using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface ICaseActivityRepository
    {
        Task<IEnumerable<CaseActivity>> GetByCaseIdAsync(int caseId);
        Task<CaseActivity?> GetAsync(int id);
        Task<CaseActivity> AddAsync(CaseActivity activity);
        Task UpdateAsync(CaseActivity activity);
        Task DeleteAsync(int id);
    }
}
