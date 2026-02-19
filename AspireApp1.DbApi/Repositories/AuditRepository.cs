using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly ProjectDbContext _context;

    public AuditRepository(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetFilteredAsync(
        int? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= toDate.Value);
        }

        return await query
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<AuditLog?> GetAsync(int id)
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<int> DeleteExpiredAsync(DateTime beforeDate)
    {
        var expiredLogs = await _context.AuditLogs
            .Where(a => a.RetentionUntil < beforeDate)
            .ToListAsync();

        _context.AuditLogs.RemoveRange(expiredLogs);
        await _context.SaveChangesAsync();

        return expiredLogs.Count;
    }
}
