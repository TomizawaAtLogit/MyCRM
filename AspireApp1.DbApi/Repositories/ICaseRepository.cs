using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface ICaseRepository
    {
        Task<IEnumerable<Case>> GetAllAsync();
        Task<IEnumerable<Case>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Case>> GetByAssignedUserIdAsync(int userId);
        Task<IEnumerable<Case>> GetByStatusAsync(CaseStatus status);
        Task<IEnumerable<Case>> GetOverdueCasesAsync();
        Task<Case?> GetAsync(int id);
        Task<Case> AddAsync(Case caseEntity);
        Task UpdateAsync(Case caseEntity);
        Task DeleteAsync(int id);
    }
}
