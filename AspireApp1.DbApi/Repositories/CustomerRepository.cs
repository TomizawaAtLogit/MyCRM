using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ProjectDbContext _db;

    public CustomerRepository(ProjectDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _db.Customers.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Customer?> GetAsync(int id)
    {
        return await _db.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetWithChildrenAsync(int id)
    {
        return await _db.Customers
            .Include(c => c.Databases)
            .Include(c => c.Sites)
            .Include(c => c.Systems)
            .Include(c => c.Orders)
            .Include(c => c.ProjectActivities)
                .ThenInclude(pa => pa.Project)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer != null)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
        }
    }

    // Database operations
    public async Task<CustomerDatabase> AddDatabaseAsync(CustomerDatabase database)
    {
        _db.CustomerDatabases.Add(database);
        await _db.SaveChangesAsync();
        return database;
    }

    public async Task UpdateDatabaseAsync(CustomerDatabase database)
    {
        database.UpdatedAt = DateTime.UtcNow;
        _db.CustomerDatabases.Update(database);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteDatabaseAsync(int id)
    {
        var database = await _db.CustomerDatabases.FindAsync(id);
        if (database != null)
        {
            _db.CustomerDatabases.Remove(database);
            await _db.SaveChangesAsync();
        }
    }

    // Site operations
    public async Task<CustomerSite> AddSiteAsync(CustomerSite site)
    {
        _db.CustomerSites.Add(site);
        await _db.SaveChangesAsync();
        return site;
    }

    public async Task UpdateSiteAsync(CustomerSite site)
    {
        site.UpdatedAt = DateTime.UtcNow;
        _db.CustomerSites.Update(site);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSiteAsync(int id)
    {
        var site = await _db.CustomerSites.FindAsync(id);
        if (site != null)
        {
            _db.CustomerSites.Remove(site);
            await _db.SaveChangesAsync();
        }
    }

    // System operations
    public async Task<CustomerSystem> AddSystemAsync(CustomerSystem system)
    {
        _db.CustomerSystems.Add(system);
        await _db.SaveChangesAsync();
        return system;
    }

    public async Task UpdateSystemAsync(CustomerSystem system)
    {
        system.UpdatedAt = DateTime.UtcNow;
        _db.CustomerSystems.Update(system);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSystemAsync(int id)
    {
        var system = await _db.CustomerSystems.FindAsync(id);
        if (system != null)
        {
            _db.CustomerSystems.Remove(system);
            await _db.SaveChangesAsync();
        }
    }

    // Order operations
    public async Task<CustomerOrder> AddOrderAsync(CustomerOrder order)
    {
        _db.CustomerOrders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task UpdateOrderAsync(CustomerOrder order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _db.CustomerOrders.Update(order);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _db.CustomerOrders.FindAsync(id);
        if (order != null)
        {
            _db.CustomerOrders.Remove(order);
            await _db.SaveChangesAsync();
        }
    }

    // Project Activity operations
    public async Task<ProjectActivity> AddProjectActivityAsync(ProjectActivity projectActivity)
    {
        _db.ProjectActivities.Add(projectActivity);
        await _db.SaveChangesAsync();
        return projectActivity;
    }

    public async Task UpdateProjectActivityAsync(ProjectActivity projectActivity)
    {
        projectActivity.UpdatedAt = DateTime.UtcNow;
        _db.ProjectActivities.Update(projectActivity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteProjectActivityAsync(int id)
    {
        var projectActivity = await _db.ProjectActivities.FindAsync(id);
        if (projectActivity != null)
        {
            _db.ProjectActivities.Remove(projectActivity);
            await _db.SaveChangesAsync();
        }
    }
}
