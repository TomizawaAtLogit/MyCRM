namespace Ligot.DbApi.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CustomerId { get; set; }
        public int? SystemId { get; set; }
        public int? SystemComponentId { get; set; }
        public int? CustomerSiteId { get; set; }
        public int? CustomerOrderId { get; set; }
        public CaseStatus Status { get; set; } = CaseStatus.Open;
        public CasePriority Priority { get; set; } = CasePriority.Medium;
        public IssueType IssueType { get; set; } = IssueType.Question;
        public int? AssignedToUserId { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // SLA tracking
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? SlaDeadline { get; set; }

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public Models.System? System { get; set; }
        public SystemComponent? SystemComponent { get; set; }
        public CustomerSite? CustomerSite { get; set; }
        public CustomerOrder? CustomerOrder { get; set; }
        public User? AssignedToUser { get; set; }
    }
}

