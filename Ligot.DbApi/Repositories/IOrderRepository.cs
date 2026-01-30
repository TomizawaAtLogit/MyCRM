using Ligot.DbApi.Models;

namespace Ligot.DbApi.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<CustomerOrder>> GetAllAsync();
    Task<IEnumerable<CustomerOrder>> GetAllAsync(int[]? allowedCustomerIds);
    Task<CustomerOrder?> GetAsync(int id);
    Task<CustomerOrder> AddAsync(CustomerOrder order);
    Task UpdateAsync(CustomerOrder order);
    Task DeleteAsync(int id);
}

