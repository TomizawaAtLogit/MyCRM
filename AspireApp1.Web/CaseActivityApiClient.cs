using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class CaseActivityApiClient
    {
        private readonly HttpClient _http;

        public CaseActivityApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<CaseActivityDto[]?> GetByCaseIdAsync(int caseId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<CaseActivityDto[]>($"/api/caseactivities/case/{caseId}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<CaseActivityDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<CaseActivityDto>($"/api/caseactivities/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<CaseActivityDto?> CreateAsync(CaseActivityCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/caseactivities", dto, cancellationToken);
                if (res.IsSuccessStatusCode)
                    return await res.Content.ReadFromJsonAsync<CaseActivityDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        public async Task<bool> UpdateAsync(int id, CaseActivityDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/caseactivities/{id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/caseactivities/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    public record CaseActivityDto(
        int Id,
        int CaseId,
        DateTime ActivityDate,
        string Summary,
        string? Description,
        string? NextAction,
        string? ActivityType,
        string? PerformedBy,
        int? PreviousAssignedToUserId,
        int? NewAssignedToUserId,
        DateTime? CreatedAt);

    public record CaseActivityCreateDto(
        int CaseId,
        DateTime ActivityDate,
        string Summary,
        string? Description,
        string? NextAction,
        string? ActivityType,
        string? PerformedBy,
        int? PreviousAssignedToUserId = null,
        int? NewAssignedToUserId = null);
}
