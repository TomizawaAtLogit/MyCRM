using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface IPreSalesProposalRepository
    {
        Task<IEnumerable<PreSalesProposal>> GetAllAsync();
        Task<IEnumerable<PreSalesProposal>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<PreSalesProposal>> GetByAssignedUserIdAsync(int userId);
        Task<IEnumerable<PreSalesProposal>> GetByStatusAsync(PreSalesStatus status);
        Task<IEnumerable<PreSalesProposal>> GetByStageAsync(PreSalesStage stage);
        Task<PreSalesProposal?> GetAsync(int id);
        Task<PreSalesProposal> AddAsync(PreSalesProposal entity);
        Task UpdateAsync(PreSalesProposal entity);
        Task DeleteAsync(int id);
    }
}
