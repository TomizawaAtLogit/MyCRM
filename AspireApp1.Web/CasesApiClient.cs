using System.Net.Http.Json;

namespace AspireApp1.Web
{
    public class CasesApiClient
    {
        private readonly HttpClient _http;

        public CasesApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<CaseDto[]> GetCasesAsync(
            int? customerId = null, 
            CaseStatus? status = null,
            int? assignedToUserId = null,
            CasePriority? priority = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryString = "";
                var queryParams = new List<string>();
                
                if (customerId.HasValue)
                    queryParams.Add($"customerId={customerId.Value}");
                
                if (status.HasValue)
                    queryParams.Add($"status={status.Value}");
                
                if (assignedToUserId.HasValue)
                    queryParams.Add($"assignedToUserId={assignedToUserId.Value}");
                
                if (priority.HasValue)
                    queryParams.Add($"priority={priority.Value}");
                
                if (queryParams.Count > 0)
                    queryString = "?" + string.Join("&", queryParams);
                
                return await _http.GetFromJsonAsync<CaseDto[]>($"/api/cases{queryString}", cancellationToken) ?? Array.Empty<CaseDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<CaseDto>();
            }
        }

        public async Task<CaseDto[]> GetOverdueCasesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<CaseDto[]>("/api/cases/overdue", cancellationToken) ?? Array.Empty<CaseDto>();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<CaseDto>();
            }
        }

        public async Task<CaseDto?> GetCaseAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _http.GetFromJsonAsync<CaseDto>($"/api/cases/{id}", cancellationToken);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<CaseDto?> CreateCaseAsync(CaseCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("/api/cases", dto, cancellationToken);
                if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<CaseDto>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        public async Task<bool> UpdateCaseAsync(int id, CaseUpdateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"/api/cases/{id}", dto, cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCaseAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await _http.DeleteAsync($"/api/cases/{id}", cancellationToken);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    public record CaseDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        string? CustomerName,
        CaseStatus Status,
        CasePriority Priority,
        IssueType IssueType,
        int? AssignedToUserId,
        string? AssignedToUserName,
        string? ResolutionNotes,
        DateTime? DueDate,
        DateTime? ResolvedAt,
        DateTime? ClosedAt,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? FirstResponseAt,
        DateTime? SlaDeadline);

    public record CaseCreateDto(
        string Title,
        string? Description,
        int CustomerId,
        CaseStatus Status = CaseStatus.Open,
        CasePriority Priority = CasePriority.Medium,
        IssueType IssueType = IssueType.Question,
        int? AssignedToUserId = null,
        string? ResolutionNotes = null,
        DateTime? DueDate = null);

    public record CaseUpdateDto(
        int Id,
        string Title,
        string? Description,
        int CustomerId,
        CaseStatus Status,
        CasePriority Priority,
        IssueType IssueType,
        int? AssignedToUserId,
        string? ResolutionNotes,
        DateTime? DueDate);

    public enum CaseStatus
    {
        Open,
        InProgress,
        Pending,
        Resolved,
        Closed
    }

    public enum CasePriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum IssueType
    {
        Bug,
        Incident,
        ServiceRequest,
        Question,
        Maintenance
    }
}
