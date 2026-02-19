namespace AspireApp1.DbApi.DTOs;

public record EntityFileDto(
    int Id,
    string EntityType,
    int EntityId,
    string FileName,
    string OriginalFileName,
    long FileSizeBytes,
    string ContentType,
    string? Description,
    string[]? Tags,
    bool HasThumbnail,
    DateTime UploadedAt,
    string UploadedBy,
    DateTime? LastAccessedAt,
    int AccessCount,
    DateTime? RetentionUntil,
    bool IsCompressed,
    long? OriginalSizeBytes
);

public record EntityFileUploadDto(
    string EntityType,
    int EntityId,
    string? Description,
    string[]? Tags
);

public record EntityFileUpdateDto(
    string? Description,
    string[]? Tags
);

public record FileQuotaDto(
    string EntityType,
    int EntityId,
    long UsedBytes,
    long QuotaBytes,
    int FileCount
);

public record QuotaIncreaseRequestDto(
    string EntityType,
    int EntityId,
    long RequestedQuotaBytes,
    string Reason
);
