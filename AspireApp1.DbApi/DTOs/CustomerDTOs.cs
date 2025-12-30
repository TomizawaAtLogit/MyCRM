using System.ComponentModel.DataAnnotations;

namespace AspireApp1.DbApi.DTOs;

public record CustomerSiteCreateDto(
    [Required] string SiteName,
    string? Address,
    string? PostCode,
    string? Country,
    string? ContactPerson,
    string? Phone,
    string? Description);

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
    DateTime? OrderDate,
    string? Status,
    decimal? Amount,
    string? Description);
