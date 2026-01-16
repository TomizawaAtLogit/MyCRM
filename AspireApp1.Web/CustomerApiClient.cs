using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace AspireApp1.Web;

// DTOs
public record CustomerDto(
    int Id, 
    string Name, 
    string? ContactPerson, 
    string? Email, 
    string? Phone, 
    string? Address, 
    DateTime CreatedAt);

public record CustomerWithChildrenDto(
    int Id,
    string Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    DateTime CreatedAt,
    List<CustomerDatabaseDto> Databases,
    List<CustomerSiteDto> Sites,
    List<SystemDto> Systems,
    List<CustomerOrderDto> Orders,
    List<CustomerProjectActivityDto> ProjectActivities);

public record CustomerCreateDto(
    [Required] string Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address);

public record CustomerUpdateDto(
    int Id,
    [Required] string Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address);

public record CustomerDatabaseDto(
    int Id,
    int CustomerId,
    string DatabaseName,
    string? DatabaseType,
    string? ServerName,
    string? Port,
    string? Version,
    string? Description);

public record CustomerDatabaseCreateDto(
    [Required] string DatabaseName,
    string? DatabaseType,
    string? ServerName,
    string? Port,
    string? Version,
    string? Description);

public record CustomerSiteDto(
    int Id,
    int CustomerId,
    string SiteName,
    string? Address,
    string? PostCode,
    string? Country,
    string? ContactPerson,
    string? Phone,
    string? Description,
    double? Latitude,
    double? Longitude);

public record CustomerSiteCreateDto(
    [Required] string SiteName,
    string? Address,
    string? PostCode,
    string? Country,
    string? ContactPerson,
    string? Phone,
    string? Description,
    double? Latitude,
    double? Longitude);

public record CustomerSiteUpdateDto(
    [Required] string SiteName,
    string? Address,
    string? PostCode,
    string? Country,
    string? ContactPerson,
    string? Phone,
    string? Description,
    double? Latitude,
    double? Longitude);

public record CustomerSystemDto(
    int Id,
    int CustomerId,
    string SystemName,
    string? ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? InstallationDate,
    DateTime? WarrantyExpiration,
    string? Description);

public record CustomerSystemCreateDto(
    [Required] string SystemName,
    string? ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? InstallationDate,
    DateTime? WarrantyExpiration,
    string? Description);

public record CustomerOrderDto(
    int Id,
    int CustomerId,
    string OrderNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

public record CustomerOrderCreateDto(
    [Required] string OrderNumber,
    [Required] string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

public record CustomerProjectActivityDto(
    int Id,
    int ProjectId,
    int? CustomerId,
    DateTime ActivityDate,
    string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy,
    string ProjectName);

public record CustomerProjectActivityCreateDto(
    int ProjectId,
    DateTime ActivityDate,
    [Required] string Summary,
    string? Description,
    string? NextAction,
    string? ActivityType,
    string? PerformedBy);

public record SystemDto(
    int Id,
    int CustomerId,
    string SystemName,
    DateTime? InstallationDate,
    string? Description,
    List<SystemComponentDto> Components);

public record SystemCreateDto(
    [Required] string SystemName,
    DateTime? InstallationDate,
    string? Description);

public record SystemComponentDto(
    int Id,
    int SystemId,
    string ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? WarrantyExpiration,
    string? Description);

public record SystemComponentCreateDto(
    [Required] string ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? WarrantyExpiration,
    string? Description);

public class CustomerApiClient
{
    private readonly HttpClient _http;

    public CustomerApiClient(HttpClient http)
    {
        _http = http;
    }

    // Customer operations
    public async Task<CustomerDto[]> GetCustomersAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<CustomerDto[]>("/api/customers", ct) 
                   ?? Array.Empty<CustomerDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<CustomerDto>();
        }
    }

    public async Task<CustomerDto?> GetCustomerAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<CustomerDto>($"/api/customers/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<CustomerWithChildrenDto?> GetCustomerWithChildrenAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync($"/api/customers/{id}/with-children", ct);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"API Error: Status {response.StatusCode}, Content: {errorContent}");
                return null;
            }
            
            return await response.Content.ReadFromJsonAsync<CustomerWithChildrenDto>(ct);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Exception: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return null;
        }
    }

    public async Task<CustomerDto?> CreateCustomerAsync(CustomerCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("/api/customers", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerDto>(ct);
        return null;
    }

    public async Task<bool> UpdateCustomerAsync(CustomerUpdateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{dto.Id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> DeleteCustomerAsync(int id, CancellationToken ct = default)
    {
        return await _http.DeleteAsync($"/api/customers/{id}", ct);
    }

    // Database operations
    public async Task<CustomerDatabaseDto?> AddDatabaseAsync(int customerId, CustomerDatabaseCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/databases", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerDatabaseDto>(ct);
        return null;
    }

    public async Task<bool> UpdateDatabaseAsync(int customerId, int id, CustomerDatabaseDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/databases/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteDatabaseAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/databases/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // Site operations
    public async Task<CustomerSiteDto?> AddSiteAsync(int customerId, CustomerSiteCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/sites", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerSiteDto>(ct);
        return null;
    }

    public async Task<bool> UpdateSiteAsync(int customerId, int id, CustomerSiteUpdateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/sites/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSiteAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/sites/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // System operations
    public async Task<CustomerSystemDto?> AddSystemAsync(int customerId, CustomerSystemCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/systems", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerSystemDto>(ct);
        return null;
    }

    public async Task<bool> UpdateSystemAsync(int customerId, int id, CustomerSystemDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/systems/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSystemAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/systems/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // New System operations (parent entity)
    public async Task<SystemDto?> AddNewSystemAsync(int customerId, SystemCreateDto dto, CancellationToken ct = default)
    {
        try
        {
            Console.WriteLine($"Attempting to add system: {dto.SystemName} for customer {customerId}");
            var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/new-systems", dto, ct);
            Console.WriteLine($"Response status: {res.StatusCode}");
            
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<SystemDto>(ct);
                Console.WriteLine($"Successfully added system with ID: {result?.Id}");
                return result;
            }
            
            var errorContent = await res.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"API Error adding system: Status {res.StatusCode}, Content: {errorContent}");
            throw new HttpRequestException($"Failed to add system: {res.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception adding system: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw new Exception($"Unexpected error adding system: {ex.Message}", ex);
        }
    }

    public async Task<SystemDto?> GetSystemWithComponentsAsync(int customerId, int systemId, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<SystemDto>($"/api/customers/{customerId}/new-systems/{systemId}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> UpdateNewSystemAsync(int customerId, int id, SystemDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/new-systems/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteNewSystemAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/new-systems/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // SystemComponent operations (child entity)
    public async Task<SystemComponentDto?> AddSystemComponentAsync(int customerId, int systemId, SystemComponentCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/new-systems/{systemId}/components", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<SystemComponentDto>(ct);
        return null;
    }

    public async Task<bool> UpdateSystemComponentAsync(int customerId, int systemId, int id, SystemComponentDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/new-systems/{systemId}/components/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSystemComponentAsync(int customerId, int systemId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/new-systems/{systemId}/components/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // Order operations
    public async Task<CustomerOrderDto?> AddOrderAsync(int customerId, CustomerOrderCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/orders", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerOrderDto>(ct);
        return null;
    }

    public async Task<bool> UpdateOrderAsync(int customerId, int id, CustomerOrderDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/orders/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/orders/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // Project Activity operations
    public async Task<CustomerProjectActivityDto?> AddProjectActivityAsync(int customerId, CustomerProjectActivityCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"/api/customers/{customerId}/project-activities", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerProjectActivityDto>(ct);
        return null;
    }

    public async Task<bool> UpdateProjectActivityAsync(int customerId, int id, CustomerProjectActivityDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/customers/{customerId}/project-activities/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProjectActivityAsync(int customerId, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/customers/{customerId}/project-activities/{id}", ct);
        return res.IsSuccessStatusCode;
    }
}
