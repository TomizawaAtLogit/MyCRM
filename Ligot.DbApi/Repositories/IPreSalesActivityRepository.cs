using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories
{
    public interface IPreSalesActivityRepository
    {
        Task<IEnumerable<PreSalesActivity>> GetAllAsync();
        Task<IEnumerable<PreSalesActivity>> GetAllAsync(int[]? allowedCustomerIds);
        Task<IEnumerable<PreSalesActivity>> GetByProposalIdAsync(int proposalId);
        Task<PreSalesActivity?> GetAsync(int id);
        Task<PreSalesActivity> AddAsync(PreSalesActivity entity);
        Task UpdateAsync(PreSalesActivity entity);
        Task DeleteAsync(int id);
    }
}

