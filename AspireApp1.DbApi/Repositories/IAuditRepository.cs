using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface IAuditRepository
{
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AuditLog>> GetFilteredAsync(
        int? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<AuditLog?> GetAsync(int id);
    Task<AuditLog> AddAsync(AuditLog auditLog);
    Task<int> DeleteExpiredAsync(DateTime beforeDate);
}
