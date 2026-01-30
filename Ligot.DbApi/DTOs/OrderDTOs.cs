namespace Ligot.DbApi.DTOs;

/// <summary>
/// DTO for OrderWithCustomer response that includes customer information
/// </summary>
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

/// <summary>
/// DTO for creating a new order
/// </summary>
public record OrderCreateDto(
    int CustomerId,
    string OrderNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

/// <summary>
/// DTO for updating an existing order
/// </summary>
public record OrderUpdateDto(
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

