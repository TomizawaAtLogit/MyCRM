using System.ComponentModel.DataAnnotations;

namespace AspireApp1.DbApi.DTOs;

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

public record CustomerDatabaseCreateDto(
    [Required] string DatabaseName,
    string? DatabaseType,
    string? ServerName,
    string? Port,
    string? Version,
    string? Description);

public record CustomerSystemCreateDto(
    [Required] string SystemName,
    string? ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
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

public record CustomerOrderUpdateDto(
    [Required] string OrderNumber,
    [Required] string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? ContractValue,
    string? BillingFrequency,
    string? Status,
    string? Description);

public record SystemCreateDto(
    [Required] string SystemName,
    DateTime? InstallationDate,
    string? Description);

public record SystemComponentCreateDto(
    [Required] string ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? WarrantyExpiration,
    string? Description);

public record SystemDto(
    int Id,
    int CustomerId,
    [Required] string SystemName,
    DateTime? InstallationDate,
    string? Description,
    List<SystemComponentDto> Components);

public record SystemComponentDto(
    int Id,
    int SystemId,
    [Required] string ComponentType,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    DateTime? WarrantyExpiration,
    string? Description);
