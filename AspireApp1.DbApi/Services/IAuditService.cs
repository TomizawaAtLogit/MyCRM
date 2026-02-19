using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Services;

public interface IAuditService
{
    Task LogActionAsync(string username, int? userId, string action, string entityType, int? entityId, object? entitySnapshot = null);
}
