using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;

namespace AspireApp1.Web
{
    public class PreSalesWorkHoursApiClient
    {
        private readonly HttpClient _http;

        public PreSalesWorkHoursApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<PreSalesWorkHourDto[]> GetWorkHoursAsync(
            int? proposalId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryString = "";
                if (proposalId.HasValue)
                    queryString = $"?proposalId={proposalId.Value}";
                
                return await _http.GetFromJsonAsync<PreSalesWorkHourDto[]>($"/api/presalesworkhours{queryString}", cancellationToken) ?? Array.Empty<PreSalesWorkHourDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<PreSalesWorkHourDto>();
            }
        }

        public async Task<PreSalesWorkHourDto?> GetWorkHourAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<PreSalesWorkHourDto>($"/api/presalesworkhours/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<PreSalesWorkHourDto?> CreateWorkHourAsync(CreatePreSalesWorkHourDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/presalesworkhours", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<PreSalesWorkHourDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException) { }
            return null;
        }

        public async Task<bool> UpdateWorkHourAsync(UpdatePreSalesWorkHourDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/presalesworkhours/{dto.Id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteWorkHourAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/presalesworkhours/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    // DTOs
    public record PreSalesWorkHourDto(
        int Id,
        int PreSalesProposalId,
        string Title,
        string? Description,
        int NumberOfPeople,
        decimal WorkingHours,
        decimal HourlyWage,
        decimal TotalCost,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreatePreSalesWorkHourDto(
        int PreSalesProposalId,
        [Required] string Title,
        string? Description = null,
        [Required] [Range(1, int.MaxValue, ErrorMessage = "Number of people must be at least 1")]
        int NumberOfPeople = 1,
        [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Working hours must be greater than 0")]
        decimal WorkingHours = 1,
        [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Hourly wage must be greater than 0")]
        decimal HourlyWage = 0);

    public record UpdatePreSalesWorkHourDto(
        int Id,
        int PreSalesProposalId,
        [Required] string Title,
        string? Description,
        [Required] [Range(1, int.MaxValue, ErrorMessage = "Number of people must be at least 1")]
        int NumberOfPeople,
        [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Working hours must be greater than 0")]
        decimal WorkingHours,
        [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Hourly wage must be greater than 0")]
        decimal HourlyWage);
}
