using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class PreSalesActivitiesApiClient
    {
        private readonly HttpClient _http;

        public PreSalesActivitiesApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<PreSalesActivityDto[]> GetActivitiesAsync(
            int? proposalId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryString = "";
                if (proposalId.HasValue)
                    queryString = $"?proposalId={proposalId.Value}";
                
                return await _http.GetFromJsonAsync<PreSalesActivityDto[]>($"/api/presalesactivities{queryString}", cancellationToken) ?? Array.Empty<PreSalesActivityDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<PreSalesActivityDto>();
            }
        }

        public async Task<PreSalesActivityDto?> GetActivityAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<PreSalesActivityDto>($"/api/presalesactivities/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<PreSalesActivityDto?> CreateActivityAsync(CreatePreSalesActivityDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/presalesactivities", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<PreSalesActivityDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException) { }
            return null;
        }

        public async Task<bool> UpdateActivityAsync(UpdatePreSalesActivityDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/presalesactivities/{dto.Id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteActivityAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/presalesactivities/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    // DTOs
    public record PreSalesActivityDto(
        int Id,
        int PreSalesProposalId,
        DateTime ActivityDate,
        string Summary,
        string? Description,
        string? NextAction,
        string? ActivityType,
        string? PerformedBy,
        int? PreviousAssignedToUserId,
        int? NewAssignedToUserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreatePreSalesActivityDto(
        int PreSalesProposalId,
        string Summary,
        string? Description = null,
        string? NextAction = null,
        string? ActivityType = null,
        string? PerformedBy = null,
        DateTime? ActivityDate = null);

    public record UpdatePreSalesActivityDto(
        int Id,
        int PreSalesProposalId,
        string Summary,
        string? Description,
        string? NextAction,
        string? ActivityType,
        DateTime ActivityDate);
}
