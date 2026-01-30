using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Ligot.Web;

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
            
            var response = await _http.GetAsync($"/api/audits{query}", ct);
            
            _logger.LogInformation("Audit logs API response: {StatusCode}", response.StatusCode);
            
            // Handle 400 BadRequest or 403 Forbidden - user not in database or no permissions
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Access denied when fetching audit logs: {StatusCode} - Response: {Content}. User may not be registered in the system.",
                    response.StatusCode, content);
                return Array.Empty<AuditLogDto>();
            }
            
            // Handle other non-success status codes
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Error fetching audit logs: {StatusCode} - {Content}", 
                    response.StatusCode, content);
                return Array.Empty<AuditLogDto>();
            }
            
            var logs = await response.Content.ReadFromJsonAsync<AuditLogDto[]>(ct);
            return logs ?? Array.Empty<AuditLogDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching audit logs: {Message}", ex.Message);
            return Array.Empty<AuditLogDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching audit logs: {Message}", ex.Message);
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
            var response = await _http.GetAsync($"/api/audits/is-admin", ct);
            
            _logger.LogInformation("IsAdmin API response: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                // Log as warning for expected auth failures, not errors
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Access denied checking admin status: {StatusCode} - Response: {Content}. User may not be registered.", 
                        response.StatusCode, content);
                }
                else
                {
                    _logger.LogError("Error checking admin status: {StatusCode} - Response: {Content}", 
                        response.StatusCode, content);
                }
                return false;
            }
            
            var result = await response.Content.ReadFromJsonAsync<bool>(ct);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error checking admin status: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error checking admin status: {Message}", ex.Message);
            return false;
        }
    }
}

