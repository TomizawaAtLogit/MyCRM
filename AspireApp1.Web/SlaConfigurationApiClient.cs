using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace AspireApp1.Web;

// DTOs
public record SlaConfigurationDto(
    int Id,
    CasePriority Priority,
    int ResponseTimeHours,
    int ResolutionTimeHours,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record UpdateSlaConfigurationDto(
    [Required] CasePriority Priority,
    [Required] int ResponseTimeHours,
    [Required] int ResolutionTimeHours);

public record UpdateAllSlaConfigurationsDto(
    [Required] UpdateSlaConfigurationDto[] Configurations);

// Mutable edit model for UI binding
public class SlaEditModel
{
    public int Id { get; set; }
    public CasePriority Priority { get; set; }
    public int ResponseTimeHours { get; set; }
    public int ResolutionTimeHours { get; set; }
}

public class SlaConfigurationApiClient
{
    private readonly HttpClient _http;

    public SlaConfigurationApiClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    public async Task<IEnumerable<SlaConfigurationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<IEnumerable<SlaConfigurationDto>>("/api/slaconfiguration", cancellationToken) ?? new List<SlaConfigurationDto>();
        }
        catch (HttpRequestException)
        {
            return new List<SlaConfigurationDto>();
        }
    }

    public async Task UpdateAllAsync(UpdateAllSlaConfigurationsDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("/api/slaconfiguration/bulk", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Failed to update SLA configurations", ex);
        }
    }
}
