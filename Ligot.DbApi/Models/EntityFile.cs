namespace Ligot.DbApi.Models;

public class EntityFile
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty; // "Customer" or "Project"
    public int EntityId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; } // JSON array of tags
    public string? ThumbnailPath { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime? LastAccessedAt { get; set; }
    public int AccessCount { get; set; } = 0;
    public DateTime? RetentionUntil { get; set; }
    public bool IsCompressed { get; set; } = false;
    public long? OriginalSizeBytes { get; set; }
}

