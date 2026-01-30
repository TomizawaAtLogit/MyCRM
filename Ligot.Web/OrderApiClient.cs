using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace Ligot.Web;

// DTOs for Order with customer information
public record OrderWithCustomerDto(
    int Id,
    int CustomerId,
    string CustomerName,
    string OrderNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description,
    DateTime CreatedAt);

public record OrderCreateDto(
    int CustomerId,
    [Required] string OrderNumber,
    [Required] string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

public record OrderUpdateDto(
    int Id,
    int CustomerId,
    [Required] string OrderNumber,
    [Required] string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

public class OrderApiClient
{
    private readonly HttpClient _http;

    public OrderApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<OrderWithCustomerDto[]> GetOrdersAsync(CancellationToken ct = default)
    {
        try
        {
            var orders = await _http.GetFromJsonAsync<CustomerOrderWithCustomerDto[]>("/api/orders", ct);
            if (orders == null) return Array.Empty<OrderWithCustomerDto>();

            // Map to OrderWithCustomerDto with customer name
            return orders.Select(o => new OrderWithCustomerDto(
                o.Id,
                o.CustomerId,
                o.Customer?.Name ?? "Unknown",
                o.OrderNumber,
                o.ContractType,
                o.StartDate,
                o.EndDate,
                o.ContractValue,
                o.BillingFrequency,
                o.Status,
                o.Description,
                o.CreatedAt
            )).ToArray();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<OrderWithCustomerDto>();
        }
    }

    public async Task<CustomerOrderDto?> GetOrderAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<CustomerOrderDto>($"/api/orders/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<CustomerOrderDto?> CreateOrderAsync(OrderCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("/api/orders", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<CustomerOrderDto>(ct);
        return null;
    }

    public async Task<bool> UpdateOrderAsync(OrderUpdateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/orders/{dto.Id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/orders/{id}", ct);
        return res.IsSuccessStatusCode;
    }
}

// Helper DTO to match the API response structure from backend
internal record CustomerOrderWithCustomerDto(
    int Id,
    int CustomerId,
    string OrderNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description,
    DateTime CreatedAt,
    CustomerDtoSimple? Customer);

internal record CustomerDtoSimple(string Name);

