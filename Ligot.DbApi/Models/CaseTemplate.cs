namespace Ligot.DbApi.Models;

public class CaseTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IssueType IssueType { get; set; }
    public CasePriority DefaultPriority { get; set; } = CasePriority.Medium;
    public string TitleTemplate { get; set; } = string.Empty;
    public string? DescriptionTemplate { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

