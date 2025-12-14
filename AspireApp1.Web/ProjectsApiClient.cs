using System.Net.Http.Json;

namespace AspireApp1.Web;

public class ProjectsApiClient(HttpClient httpClient)
{
    private readonly HttpClient _http = httpClient;

    public async Task<ProjectDto[]> GetProjectsAsync(CancellationToken cancellationToken = default) =>
        await _http.GetFromJsonAsync<ProjectDto[]>("/api/projects", cancellationToken) ?? Array.Empty<ProjectDto>();

    public async Task<ProjectDto?> GetProjectAsync(int id, CancellationToken cancellationToken = default) =>
        await _http.GetFromJsonAsync<ProjectDto>($"/api/projects/{id}", cancellationToken);

    public async Task<ProjectDto?> CreateProjectAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        var res = await _http.PostAsJsonAsync("/api/projects", dto, cancellationToken);
        if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<ProjectDto>(cancellationToken: cancellationToken);
        return null;
    }

    public async Task<bool> UpdateProjectAsync(int id, ProjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/projects/{id}", dto, cancellationToken);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        var res = await _http.DeleteAsync($"/api/projects/{id}", cancellationToken);
        return res.IsSuccessStatusCode;
    }
}

public record ProjectDto(int Id, string Name, string? Description, DateTime CreatedAt);
public record ProjectCreateDto(string Name, string? Description);
