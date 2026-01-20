using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ProjectDbContext _db;

    public OrderRepository(ProjectDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CustomerOrder>> GetAllAsync()
    {
        return await _db.CustomerOrders
            .Include(o => o.Customer)
            .AsNoTracking()
            .OrderByDescending(o => o.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<CustomerOrder>> GetAllAsync(int[]? allowedCustomerIds)
    {
        // If allowedCustomerIds is null, return all orders (unrestricted access)
        if (allowedCustomerIds == null)
        {
            return await GetAllAsync();
        }

        // If allowedCustomerIds is empty, return no orders
        if (allowedCustomerIds.Length == 0)
        {
            return Enumerable.Empty<CustomerOrder>();
        }

        // Return only orders for allowed customers
        return await _db.CustomerOrders
            .Include(o => o.Customer)
            .Where(o => allowedCustomerIds.Contains(o.CustomerId))
            .AsNoTracking()
            .OrderByDescending(o => o.StartDate)
            .ToListAsync();
    }

    public async Task<CustomerOrder?> GetAsync(int id)
    {
        return await _db.CustomerOrders
            .Include(o => o.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<CustomerOrder> AddAsync(CustomerOrder order)
    {
        _db.CustomerOrders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(CustomerOrder order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _db.CustomerOrders.Update(order);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _db.CustomerOrders.FindAsync(id);
        if (order != null)
        {
            _db.CustomerOrders.Remove(order);
            await _db.SaveChangesAsync();
        }
    }
}
