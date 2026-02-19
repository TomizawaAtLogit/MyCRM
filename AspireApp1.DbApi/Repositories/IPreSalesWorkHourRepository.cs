using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories
{
    public interface IPreSalesWorkHourRepository
    {
        Task<IEnumerable<PreSalesWorkHour>> GetAllAsync();
        Task<IEnumerable<PreSalesWorkHour>> GetAllAsync(int[]? allowedCustomerIds);
        Task<IEnumerable<PreSalesWorkHour>> GetByProposalIdAsync(int proposalId);
        Task<PreSalesWorkHour?> GetAsync(int id);
        Task<PreSalesWorkHour> AddAsync(PreSalesWorkHour entity);
        Task UpdateAsync(PreSalesWorkHour entity);
        Task DeleteAsync(int id);
    }
}
