using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AspireApp1.Web;

// DTOs for Audit logs
public record AuditLogDto(
    int Id,
    DateTime Timestamp,
    int? UserId,
    string Username,
    string Action,
    string EntityType,
    int? EntityId,
    string? EntitySnapshot,
    DateTime RetentionUntil,
    UserDtoSimple? User);

public record UserDtoSimple(
    int Id,
    string WindowsUsername,
    string DisplayName);

public class AuditApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AuditApiClient> _logger;

    public AuditApiClient(HttpClient http, ILogger<AuditApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<AuditLogDto[]> GetAuditLogsAsync(
        bool? viewAll = null,
        string? entityType = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (viewAll.HasValue)
                queryParams.Add($"viewAll={viewAll.Value}");
            
            if (!string.IsNullOrWhiteSpace(entityType))
                queryParams.Add($"entityType={Uri.EscapeDataString(entityType)}");
            
            if (!string.IsNullOrWhiteSpace(action))
                queryParams.Add($"action={Uri.EscapeDataString(action)}");
            
            if (fromDate.HasValue)
                queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            
            if (toDate.HasValue)
                queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

            var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var logs = await _http.GetFromJsonAsync<AuditLogDto[]>($"/api/audits{query}", ct);
            return logs ?? Array.Empty<AuditLogDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching audit logs");
            return Array.Empty<AuditLogDto>();
        }
    }

    public async Task<AuditLogDto?> GetAuditLogAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<AuditLogDto>($"/api/audits/{id}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching audit log {Id}", id);
            return null;
        }
    }

    public async Task<bool> IsAdminAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<bool>($"/api/audits/is-admin", ct);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error checking admin status");
            return false;
        }
    }
}
