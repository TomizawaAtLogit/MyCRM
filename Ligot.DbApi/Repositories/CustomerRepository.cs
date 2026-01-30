using Ligot.DbApi.Models;
using Ligot.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories;

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

    public async Task<IEnumerable<Customer>> GetAllAsync(int[]? allowedCustomerIds)
    {
        // If allowedCustomerIds is null, return all customers (unrestricted access)
        if (allowedCustomerIds == null)
        {
            return await GetAllAsync();
        }

        // If allowedCustomerIds is empty, return no customers
        if (allowedCustomerIds.Length == 0)
        {
            return Enumerable.Empty<Customer>();
        }

        // Return only customers in the allowed list
        return await _db.Customers
            .Where(c => allowedCustomerIds.Contains(c.Id))
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
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
            .Include(c => c.CustomerSystems)
            .Include(c => c.Systems)
                .ThenInclude(s => s.Components)
            .Include(c => c.Orders)
            .AsSplitQuery() // Use split queries to avoid cartesian explosion
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

    public async Task<CustomerDependencyType?> GetDeletionBlockerAsync(int customerId)
    {
        if (await _db.Cases.AsNoTracking().AnyAsync(c => c.CustomerId == customerId))
            return CustomerDependencyType.Case;

        if (await _db.Projects.AsNoTracking().AnyAsync(p => p.CustomerId == customerId))
            return CustomerDependencyType.Project;

        if (await _db.RequirementDefinitions.AsNoTracking().AnyAsync(r => r.CustomerId == customerId))
            return CustomerDependencyType.RequirementDefinition;

        if (await _db.PreSalesProposals.AsNoTracking().AnyAsync(p => p.CustomerId == customerId))
            return CustomerDependencyType.PreSalesProposal;

        if (await _db.RoleCoverages.AsNoTracking().AnyAsync(rc => rc.CustomerId == customerId))
            return CustomerDependencyType.RoleCoverage;

        return null;
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

    public async Task<CustomerSite?> GetSiteByIdAsync(int id)
    {
        return await _db.CustomerSites.FindAsync(id);
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

    public async Task<CustomerOrder?> GetOrderByIdAsync(int id)
    {
        return await _db.CustomerOrders.FindAsync(id);
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

    // New System operations
    public async Task<Models.System> AddNewSystemAsync(Models.System system)
    {
        system.CreatedAt = DateTime.UtcNow;
        _db.Systems.Add(system);
        await _db.SaveChangesAsync();
        return system;
    }

    public async Task<Models.System?> GetSystemWithComponentsAsync(int systemId)
    {
        return await _db.Systems
            .Include(s => s.Components)
            .FirstOrDefaultAsync(s => s.Id == systemId);
    }

    public async Task UpdateNewSystemAsync(Models.System system)
    {
        system.UpdatedAt = DateTime.UtcNow;
        _db.Systems.Update(system);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteNewSystemAsync(int id)
    {
        var system = await _db.Systems.FindAsync(id);
        if (system != null)
        {
            _db.Systems.Remove(system);
            await _db.SaveChangesAsync();
        }
    }

    // SystemComponent operations
    public async Task<SystemComponent> AddSystemComponentAsync(SystemComponent component)
    {
        component.CreatedAt = DateTime.UtcNow;
        _db.SystemComponents.Add(component);
        await _db.SaveChangesAsync();
        return component;
    }

    public async Task<SystemComponent?> GetSystemComponentByIdAsync(int id)
    {
        return await _db.SystemComponents.FindAsync(id);
    }

    public async Task UpdateSystemComponentAsync(SystemComponent component)
    {
        component.UpdatedAt = DateTime.UtcNow;
        _db.SystemComponents.Update(component);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSystemComponentAsync(int id)
    {
        var component = await _db.SystemComponents.FindAsync(id);
        if (component != null)
        {
            _db.SystemComponents.Remove(component);
            await _db.SaveChangesAsync();
        }
    }
}

