namespace AspireApp1.DbApi.Models;

public class CustomerOrder
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty; // e.g., Maintenance, Support, License
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? ContractValue { get; set; }
    public string? BillingFrequency { get; set; } // e.g., Monthly, Annually
    public string? Status { get; set; } // e.g., Active, Expired, Pending
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Customer? Customer { get; set; }
}
