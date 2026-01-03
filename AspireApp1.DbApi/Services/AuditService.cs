using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using System.Text.Json;

namespace AspireApp1.DbApi.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepo;
    private readonly ILogger<AuditService> _logger;
    private const int DefaultRetentionDays = 365;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public AuditService(IAuditRepository auditRepo, ILogger<AuditService> logger)
    {
        _auditRepo = auditRepo;
        _logger = logger;
    }

    public async Task LogActionAsync(string username, int? userId, string action, string entityType, int? entityId, object? entitySnapshot = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Username = username,
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                EntitySnapshot = entitySnapshot != null ? JsonSerializer.Serialize(entitySnapshot, JsonOptions) : null,
                Timestamp = DateTime.UtcNow,
                RetentionUntil = DateTime.UtcNow.AddDays(DefaultRetentionDays)
            };

            await _auditRepo.AddAsync(auditLog);
            _logger.LogInformation("Audit log created: {Action} on {EntityType} {EntityId} by {Username}",
                action, entityType, entityId, username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create audit log for {Action} on {EntityType} {EntityId}",
                action, entityType, entityId);
        }
    }
}
