using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class RequirementDefinitionsApiClient
    {
        private readonly HttpClient _http;

        public RequirementDefinitionsApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<RequirementDefinitionDto[]> GetRequirementDefinitionsAsync(
            int? customerId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryString = "";
                if (customerId.HasValue)
                    queryString = $"?customerId={customerId.Value}";
                
                return await _http.GetFromJsonAsync<RequirementDefinitionDto[]>($"/api/requirementdefinitions{queryString}", cancellationToken) ?? Array.Empty<RequirementDefinitionDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<RequirementDefinitionDto>();
            }
        }

        public async Task<RequirementDefinitionDto?> GetRequirementDefinitionAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<RequirementDefinitionDto>($"/api/requirementdefinitions/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<RequirementDefinitionDto?> CreateRequirementDefinitionAsync(CreateRequirementDefinitionDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/requirementdefinitions", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<RequirementDefinitionDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException) { }
            return null;
        }

        public async Task<bool> UpdateRequirementDefinitionAsync(UpdateRequirementDefinitionDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/requirementdefinitions/{dto.Id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRequirementDefinitionAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/requirementdefinitions/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    // DTOs matching the backend
    public record RequirementDefinitionDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        string? CustomerName,
        string? Category,
        string? Priority,
        string? Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreateRequirementDefinitionDto(
        string Title,
        string? Description,
        int CustomerId,
        string? Category = null,
        string? Priority = null,
        string? Status = null);

    public record UpdateRequirementDefinitionDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        string? Category,
        string? Priority,
        string? Status);
}
