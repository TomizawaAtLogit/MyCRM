using System.Net.Http.Json;

namespace Ligot.Web
{
    public class PreSalesProposalsApiClient
    {
        private readonly HttpClient _http;

        public PreSalesProposalsApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<PreSalesProposalDto[]> GetProposalsAsync(
            int? customerId = null,
            int? assignedToUserId = null,
            PreSalesStatus? status = null,
            PreSalesStage? stage = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryString = "";
                var queryParams = new List<string>();
                
                if (customerId.HasValue)
                    queryParams.Add($"customerId={customerId.Value}");
                
                if (assignedToUserId.HasValue)
                    queryParams.Add($"assignedToUserId={assignedToUserId.Value}");
                
                if (status.HasValue)
                    queryParams.Add($"status={status.Value}");
                
                if (stage.HasValue)
                    queryParams.Add($"stage={stage.Value}");
                
                if (queryParams.Count > 0)
                    queryString = "?" + string.Join("&", queryParams);
                
                return await _http.GetFromJsonAsync<PreSalesProposalDto[]>($"/api/presalesproposals{queryString}", cancellationToken) ?? Array.Empty<PreSalesProposalDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<PreSalesProposalDto>();
            }
        }

        public async Task<PreSalesProposalDto?> GetProposalAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<PreSalesProposalDto>($"/api/presalesproposals/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<PreSalesProposalDto?> CreateProposalAsync(CreatePreSalesProposalDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/presalesproposals", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<PreSalesProposalDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException) { }
            return null;
        }

        public async Task<bool> UpdateProposalAsync(UpdatePreSalesProposalDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/presalesproposals/{dto.Id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteProposalAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/presalesproposals/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    // Enums
    public enum PreSalesStatus
    {
        Draft,
        InReview,
        Pending,
        Approved,
        Rejected,
        Closed
    }

    public enum PreSalesStage
    {
        InitialContact,
        RequirementGathering,
        ProposalDevelopment,
        PresentationScheduled,
        NegotiationInProgress,
        AwaitingDecision,
        Won,
        Lost
    }

    // DTOs
    public record PreSalesProposalDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        string? CustomerName,
        int? RequirementDefinitionId,
        string? RequirementDefinitionTitle,
        int? CustomerOrderId,
        string? CustomerOrderNumber,
        PreSalesStatus Status,
        PreSalesStage Stage,
        int? AssignedToUserId,
        string? AssignedToUserName,
        decimal? EstimatedValue,
        int? ProbabilityPercentage,
        DateTime? ExpectedCloseDate,
        DateTime? ClosedAt,
        string? Notes,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreatePreSalesProposalDto(
        string Title,
        string? Description,
        int CustomerId,
        int? RequirementDefinitionId = null,
        int? CustomerOrderId = null,
        PreSalesStatus Status = PreSalesStatus.Draft,
        PreSalesStage Stage = PreSalesStage.InitialContact,
        int? AssignedToUserId = null,
        decimal? EstimatedValue = null,
        int? ProbabilityPercentage = null,
        DateTime? ExpectedCloseDate = null,
        string? Notes = null);

    public record UpdatePreSalesProposalDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        int? RequirementDefinitionId,
        int? CustomerOrderId,
        PreSalesStatus Status,
        PreSalesStage Stage,
        int? AssignedToUserId,
        decimal? EstimatedValue,
        int? ProbabilityPercentage,
        DateTime? ExpectedCloseDate,
        string? Notes);
}

