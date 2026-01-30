namespace Ligot.DbApi.Models;

public class CaseRelationship
{
    public int Id { get; set; }
    public int SourceCaseId { get; set; }
    public int RelatedCaseId { get; set; }
    public CaseRelationshipType RelationshipType { get; set; } = CaseRelationshipType.Related;
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Case SourceCase { get; set; } = null!;
    public Case RelatedCase { get; set; } = null!;
}

