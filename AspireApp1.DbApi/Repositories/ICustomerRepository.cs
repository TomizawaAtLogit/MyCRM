using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetAsync(int id);
    Task<Customer?> GetWithChildrenAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    
    // Child entity operations
    Task<CustomerDatabase> AddDatabaseAsync(CustomerDatabase database);
    Task<CustomerSite> AddSiteAsync(CustomerSite site);
    Task<CustomerSite?> GetSiteByIdAsync(int id);
    Task<CustomerSystem> AddSystemAsync(CustomerSystem system);
    Task<CustomerOrder> AddOrderAsync(CustomerOrder order);
    Task<CustomerOrder?> GetOrderByIdAsync(int id);
    Task<ProjectActivity> AddProjectActivityAsync(ProjectActivity projectActivity);
    
    Task UpdateDatabaseAsync(CustomerDatabase database);
    Task UpdateSiteAsync(CustomerSite site);
    Task UpdateSystemAsync(CustomerSystem system);
    Task UpdateOrderAsync(CustomerOrder order);
    Task UpdateProjectActivityAsync(ProjectActivity projectActivity);
    
    Task DeleteDatabaseAsync(int id);
    Task DeleteSiteAsync(int id);
    Task DeleteSystemAsync(int id);
    Task DeleteOrderAsync(int id);
    Task DeleteProjectActivityAsync(int id);

    // New System operations
    Task<Models.System> AddNewSystemAsync(Models.System system);
    Task<Models.System?> GetSystemWithComponentsAsync(int systemId);
    Task UpdateNewSystemAsync(Models.System system);
    Task DeleteNewSystemAsync(int id);

    // SystemComponent operations
    Task<SystemComponent> AddSystemComponentAsync(SystemComponent component);
    Task UpdateSystemComponentAsync(SystemComponent component);
    Task DeleteSystemComponentAsync(int id);
}
