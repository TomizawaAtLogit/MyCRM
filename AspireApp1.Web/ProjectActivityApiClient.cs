using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace AspireApp1.Web;

// DTOs
public record ProjectActivityDto(
    int Id,
    int ProjectId,
    DateTime ActivityDate,
    string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    string? ProjectName);

public record ProjectActivityCreateDto(
    int ProjectId,
    [Required] DateTime ActivityDate,
    [Required, MaxLength(500)] string Summary,
    [MaxLength(5000)] string? Description,
    [MaxLength(1000)] string? NextAction,
    [MaxLength(100)] string? ActivityType,
    [MaxLength(200)] string? PerformedBy);

public class ProjectActivityApiClient
{
    private readonly HttpClient _http;

    public ProjectActivityApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProjectActivityDto[]> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProjectActivityDto[]>("/api/projectactivities", ct) 
                   ?? Array.Empty<ProjectActivityDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<ProjectActivityDto>();
        }
    }

    public async Task<ProjectActivityDto?> GetAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProjectActivityDto>($"/api/projectactivities/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<ProjectActivityDto[]> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProjectActivityDto[]>($"/api/projectactivities/by-project/{projectId}", ct) 
                   ?? Array.Empty<ProjectActivityDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<ProjectActivityDto>();
        }
    }

    public async Task<ProjectActivityDto[]> SearchAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? customerId = null,
        string? activityType = null,
        CancellationToken ct = default)
    {
        try
        {
            var query = new List<string>();
            if (startDate.HasValue)
                query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            if (customerId.HasValue)
                query.Add($"customerId={customerId.Value}");
            if (!string.IsNullOrWhiteSpace(activityType))
                query.Add($"activityType={Uri.EscapeDataString(activityType)}");

            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
            return await _http.GetFromJsonAsync<ProjectActivityDto[]>($"/api/projectactivities{queryString}", ct) 
                   ?? Array.Empty<ProjectActivityDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<ProjectActivityDto>();
        }
    }

    public async Task<ProjectActivityDto?> CreateAsync(ProjectActivityCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("/api/projectactivities", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<ProjectActivityDto>(ct);
        return null;
    }

    public async Task<bool> UpdateAsync(int id, ProjectActivityDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/projectactivities/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/projectactivities/{id}", ct);
        return res.IsSuccessStatusCode;
    }
}
