using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class ProjectsApiClient
    {
        private readonly HttpClient _http;

        public ProjectsApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<ProjectDto[]> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProjectDto[]>("/api/projects", cancellationToken) ?? Array.Empty<ProjectDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<ProjectDto>();
            }
        }

        public async Task<ProjectDto?> GetProjectAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProjectDto>($"/api/projects/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<ProjectDto?> CreateProjectAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/projects", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<ProjectDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        public async Task<bool> UpdateProjectAsync(int id, ProjectCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/projects/{id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/projects/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    public record ProjectDto(int Id, string Name, string? Description, DateTime CreatedAt);
    public record ProjectCreateDto(string Name, string? Description);
}
