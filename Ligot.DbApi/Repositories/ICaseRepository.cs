using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories
{
    public interface ICaseRepository
    {
        Task<IEnumerable<Case>> GetAllAsync();
        Task<IEnumerable<Case>> GetAllAsync(int[]? allowedCustomerIds);
        Task<IEnumerable<Case>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Case>> GetByAssignedUserIdAsync(int userId);
        Task<IEnumerable<Case>> GetByStatusAsync(CaseStatus status);
        Task<IEnumerable<Case>> GetOverdueCasesAsync();
        Task<Case?> GetAsync(int id);
        Task<Case> AddAsync(Case caseEntity);
        Task<(int? previousAssignedToUserId, CaseStatus previousStatus)> UpdateAsync(Case caseEntity);
        Task DeleteAsync(int id);
        Task<int> GetOpenRelatedCasesCountAsync(int caseId);
        Task<int[]> BulkUpdateAssignmentAsync(int[] caseIds, int? assignedToUserId);
        Task<int[]> BulkUpdateStatusAsync(int[] caseIds, CaseStatus status);
    }
}

