namespace AspireApp1.DbApi.Models;

public class CaseRelationship
{
    public int Id { get; set; }
    public int SourceCaseId { get; set; }
    public int RelatedCaseId { get; set; }
    public string RelationshipType { get; set; } = "Related"; // Related, Blocks, Duplicate, etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Case SourceCase { get; set; } = null!;
    public Case RelatedCase { get; set; } = null!;
}
