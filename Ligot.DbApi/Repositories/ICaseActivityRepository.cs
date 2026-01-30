using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories
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

