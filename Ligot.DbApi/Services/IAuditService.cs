using Ligot.DbApi.Models;

namespace Ligot.DbApi.Services;

public interface IAuditService
{
    Task LogActionAsync(string username, int? userId, string action, string entityType, int? entityId, object? entitySnapshot = null);
}

