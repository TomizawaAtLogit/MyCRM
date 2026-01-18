using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class ProjectTaskApiClient
    {
        private readonly HttpClient _http;

        public ProjectTaskApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<ProjectTaskDto[]> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProjectTaskDto[]>($"/api/projects/{projectId}/tasks", cancellationToken) ?? Array.Empty<ProjectTaskDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<ProjectTaskDto>();
            }
        }

        public async Task<ProjectTaskDto?> GetTaskAsync(int projectId, int taskId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProjectTaskDto>($"/api/projects/{projectId}/tasks/{taskId}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<ProjectTaskDto?> CreateTaskAsync(int projectId, CreateProjectTaskDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync($"/api/projects/{projectId}/tasks", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<ProjectTaskDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        public async Task<bool> UpdateTaskAsync(int taskId, UpdateProjectTaskDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/tasks/{taskId}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int taskId, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/tasks/{taskId}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    public record ProjectTaskDto(
        int Id, 
        int ProjectId, 
        string Title, 
        string? Description, 
        DateTime StartAtUtc, 
        DateTime EndAtUtc, 
        ProjectTaskStatus Status, 
        string? PerformedBy,
        int DisplayOrder, 
        DateTime CreatedAt, 
        DateTime? UpdatedAt);

    public record CreateProjectTaskDto(
        string Title, 
        string? Description, 
        DateTime StartAtUtc, 
        DateTime EndAtUtc, 
        ProjectTaskStatus Status = ProjectTaskStatus.NotStarted,
        string? PerformedBy = null,
        int DisplayOrder = 0);

    public record UpdateProjectTaskDto(
        string Title, 
        string? Description, 
        DateTime StartAtUtc, 
        DateTime EndAtUtc, 
        ProjectTaskStatus Status,
        string? PerformedBy,
        int DisplayOrder);

    public enum ProjectTaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Blocked
    }
}
