using System.Net.Http.Json;

namespace AspireApp1.Web;

public class ProjectsApiClient(HttpClient httpClient)
{
    public async Task<ProjectDto[]> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await httpClient.GetFromJsonAsync<ProjectDto[]>("/api/projects", cancellationToken);
        return projects ?? Array.Empty<ProjectDto>();
    }
}

public record ProjectDto(int Id, string Name, string? Description, DateTime CreatedAt);
