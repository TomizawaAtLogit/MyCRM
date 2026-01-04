using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text.Json;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // DISABLED FOR LOCAL DEVELOPMENT
public class EntityFilesController : ControllerBase
{
    private readonly ProjectDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<EntityFilesController> _logger;
    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100MB
    private const int MaxBatchUploadCount = 20;
    private const long DefaultQuotaBytes = 1L * 1024 * 1024 * 1024; // 1GB per entity

    public EntityFilesController(
        ProjectDbContext context,
        IFileStorageService fileStorage,
        ILogger<EntityFilesController> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<ActionResult<IEnumerable<EntityFileDto>>> GetFiles(string entityType, int entityId)
    {
        var files = await _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();

        return Ok(files.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EntityFileDto>> GetFile(int id)
    {
        var file = await _context.EntityFiles.FindAsync(id);
        if (file == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(file));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(MaxFileSizeBytes)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<EntityFileDto>> UploadFile(
        [FromForm] string entityType,
        [FromForm] int entityId,
        IFormFile file,
        [FromForm] string? description,
        [FromForm] string? tags)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest($"File size exceeds maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)}MB");
        }

        if (!LocalFileStorageService.IsContentTypeAllowed(file.ContentType ?? ""))
        {
            return BadRequest($"File type '{file.ContentType ?? "unknown"}' is not allowed");
        }

        // Check quota
        var quotaCheck = await CheckQuotaAsync(entityType, entityId, file.Length);
        if (!quotaCheck.hasQuota)
        {
            return BadRequest($"Storage quota exceeded. Used: {quotaCheck.usedBytes / (1024 * 1024)}MB, Quota: {quotaCheck.quotaBytes / (1024 * 1024)}MB");
        }

        // Check for duplicate filename
        var existingFile = await _context.EntityFiles
            .FirstOrDefaultAsync(f => f.EntityType == entityType && 
                                     f.EntityId == entityId && 
                                     f.OriginalFileName == file.FileName);

        if (existingFile != null)
        {
            // Delete old file
            await _fileStorage.DeleteFileAsync(existingFile.StoragePath);
            if (!string.IsNullOrEmpty(existingFile.ThumbnailPath))
            {
                await _fileStorage.DeleteFileAsync(existingFile.ThumbnailPath);
            }
            _context.EntityFiles.Remove(existingFile);
        }

        var username = User.Identity?.Name ?? "Unknown";
        
        // Save file
        using var fileStream = file.OpenReadStream();
        var storagePath = await _fileStorage.SaveFileAsync(fileStream, file.FileName, entityType, entityId);

        // Generate thumbnail for images
        string? thumbnailPath = null;
        if (file.ContentType != null && file.ContentType.StartsWith("image/"))
        {
            thumbnailPath = await _fileStorage.GenerateThumbnailAsync(storagePath, file.ContentType);
        }

        // Parse tags
        string[]? tagsArray = null;
        if (!string.IsNullOrEmpty(tags))
        {
            try
            {
                tagsArray = JsonSerializer.Deserialize<string[]>(tags);
            }
            catch
            {
                tagsArray = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
        }

        // Create file record
        var entityFile = new EntityFile
        {
            EntityType = entityType,
            EntityId = entityId,
            FileName = _fileStorage.SanitizeFileName(file.FileName),
            OriginalFileName = file.FileName,
            StoragePath = storagePath,
            FileSizeBytes = file.Length,
            ContentType = file.ContentType ?? "application/octet-stream",
            Description = description,
            Tags = tagsArray != null ? JsonSerializer.Serialize(tagsArray) : null,
            ThumbnailPath = thumbnailPath,
            UploadedBy = username,
            UploadedAt = DateTime.UtcNow
        };

        _context.EntityFiles.Add(entityFile);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File uploaded: {FileName} for {EntityType} {EntityId} by {User}", 
            file.FileName, entityType, entityId, username);

        return CreatedAtAction(nameof(GetFile), new { id = entityFile.Id }, MapToDto(entityFile));
    }

    [HttpPost("upload-batch")]
    [RequestSizeLimit(MaxFileSizeBytes * MaxBatchUploadCount)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IEnumerable<EntityFileDto>>> UploadBatch(
        [FromForm] string entityType,
        [FromForm] int entityId,
        IFormFileCollection files,
        [FromForm] string? description)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("No files uploaded");
        }

        if (files.Count > MaxBatchUploadCount)
        {
            return BadRequest($"Cannot upload more than {MaxBatchUploadCount} files at once");
        }

        var totalSize = files.Sum(f => f.Length);
        if (totalSize > MaxFileSizeBytes * MaxBatchUploadCount)
        {
            return BadRequest($"Total file size exceeds maximum allowed");
        }

        // Check quota
        var quotaCheck = await CheckQuotaAsync(entityType, entityId, totalSize);
        if (!quotaCheck.hasQuota)
        {
            return BadRequest($"Storage quota exceeded");
        }

        var uploadedFiles = new List<EntityFile>();
        var username = User.Identity?.Name ?? "Unknown";

        foreach (var file in files)
        {
            if (!LocalFileStorageService.IsContentTypeAllowed(file.ContentType ?? ""))
            {
                _logger.LogWarning("Skipping file with disallowed content type: {FileName} - {ContentType}", 
                    file.FileName, file.ContentType ?? "unknown");
                continue;
            }

            try
            {
                using var fileStream = file.OpenReadStream();
                var storagePath = await _fileStorage.SaveFileAsync(fileStream, file.FileName, entityType, entityId);

                string? thumbnailPath = null;
                if (file.ContentType != null && file.ContentType.StartsWith("image/"))
                {
                    thumbnailPath = await _fileStorage.GenerateThumbnailAsync(storagePath, file.ContentType);
                }

                var entityFile = new EntityFile
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    FileName = _fileStorage.SanitizeFileName(file.FileName),
                    OriginalFileName = file.FileName,
                    StoragePath = storagePath,
                    FileSizeBytes = file.Length,
                    ContentType = file.ContentType ?? "application/octet-stream",
                    Description = description,
                    ThumbnailPath = thumbnailPath,
                    UploadedBy = username,
                    UploadedAt = DateTime.UtcNow
                };

                _context.EntityFiles.Add(entityFile);
                uploadedFiles.Add(entityFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", file.FileName);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Batch upload completed: {Count} files for {EntityType} {EntityId}", 
            uploadedFiles.Count, entityType, entityId);

        return Ok(uploadedFiles.Select(MapToDto));
    }

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var file = await _context.EntityFiles.FindAsync(id);
        if (file == null)
        {
            return NotFound();
        }

        try
        {
            var (fileStream, contentType) = await _fileStorage.GetFileAsync(file.StoragePath);

            // Update access tracking
            file.LastAccessedAt = DateTime.UtcNow;
            file.AccessCount++;
            await _context.SaveChangesAsync();

            _logger.LogInformation("File downloaded: {FileName} by {User}", file.OriginalFileName, User.Identity?.Name);

            return File(fileStream, contentType, file.OriginalFileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found in storage");
        }
    }

    [HttpGet("download-all/{entityType}/{entityId}")]
    public async Task<IActionResult> DownloadAllAsZip(string entityType, int entityId)
    {
        var files = await _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId)
            .ToListAsync();

        if (files.Count == 0)
        {
            return NotFound("No files found");
        }

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files)
            {
                try
                {
                    var (fileStream, _) = await _fileStorage.GetFileAsync(file.StoragePath);
                    var entry = archive.CreateEntry(file.OriginalFileName, CompressionLevel.Fastest);

                    using var entryStream = entry.Open();
                    await fileStream.CopyToAsync(entryStream);
                    await fileStream.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add file to ZIP: {FileName}", file.OriginalFileName);
                }
            }
        }

        memoryStream.Position = 0;
        var zipFileName = $"{entityType}_{entityId}_files_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";

        _logger.LogInformation("ZIP download: {ZipFileName} by {User}", zipFileName, User.Identity?.Name);

        return File(memoryStream, "application/zip", zipFileName);
    }

    [HttpGet("thumbnail/{id:int}")]
    public async Task<IActionResult> GetThumbnail(int id)
    {
        var file = await _context.EntityFiles.FindAsync(id);
        if (file == null || string.IsNullOrEmpty(file.ThumbnailPath))
        {
            return NotFound();
        }

        try
        {
            var (fileStream, _) = await _fileStorage.GetFileAsync(file.ThumbnailPath);
            return File(fileStream, "image/jpeg");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateFileMetadata(int id, EntityFileUpdateDto dto)
    {
        var file = await _context.EntityFiles.FindAsync(id);
        if (file == null)
        {
            return NotFound();
        }

        file.Description = dto.Description;
        file.Tags = dto.Tags != null ? JsonSerializer.Serialize(dto.Tags) : null;

        await _context.SaveChangesAsync();

        _logger.LogInformation("File metadata updated: {FileName} by {User}", file.OriginalFileName, User.Identity?.Name);

        return Ok(MapToDto(file));
    }

    [HttpPut("batch-update-metadata")]
    public async Task<IActionResult> BatchUpdateMetadata([FromBody] BatchMetadataUpdateRequest request)
    {
        var files = await _context.EntityFiles
            .Where(f => request.FileIds.Contains(f.Id))
            .ToListAsync();

        if (files.Count == 0)
        {
            return NotFound();
        }

        foreach (var file in files)
        {
            if (request.Description != null)
            {
                file.Description = request.Description;
            }
            if (request.Tags != null)
            {
                var existingTags = string.IsNullOrEmpty(file.Tags) 
                    ? new List<string>() 
                    : JsonSerializer.Deserialize<List<string>>(file.Tags) ?? new List<string>();
                
                existingTags.AddRange(request.Tags);
                file.Tags = JsonSerializer.Serialize(existingTags.Distinct());
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Batch metadata update: {Count} files by {User}", files.Count, User.Identity?.Name);

        return Ok(files.Select(MapToDto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var file = await _context.EntityFiles.FindAsync(id);
        if (file == null)
        {
            return NotFound();
        }

        // Delete physical files
        await _fileStorage.DeleteFileAsync(file.StoragePath);
        if (!string.IsNullOrEmpty(file.ThumbnailPath))
        {
            await _fileStorage.DeleteFileAsync(file.ThumbnailPath);
        }

        _context.EntityFiles.Remove(file);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File deleted: {FileName} by {User}", file.OriginalFileName, User.Identity?.Name);

        return NoContent();
    }

    [HttpGet("quota/{entityType}/{entityId}")]
    public async Task<ActionResult<FileQuotaDto>> GetQuota(string entityType, int entityId)
    {
        var files = await _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId)
            .ToListAsync();

        var usedBytes = files.Sum(f => f.FileSizeBytes);
        var fileCount = files.Count;

        return Ok(new FileQuotaDto(entityType, entityId, usedBytes, DefaultQuotaBytes, fileCount));
    }

    [HttpGet("search/{entityType}/{entityId}")]
    public async Task<ActionResult<IEnumerable<EntityFileDto>>> SearchFiles(
        string entityType, 
        int entityId,
        [FromQuery] string? query,
        [FromQuery] string? tag,
        [FromQuery] string? contentType)
    {
        var filesQuery = _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId);

        if (!string.IsNullOrEmpty(query))
        {
            filesQuery = filesQuery.Where(f => 
                f.OriginalFileName.Contains(query) || 
                (f.Description != null && f.Description.Contains(query)));
        }

        if (!string.IsNullOrEmpty(tag))
        {
            filesQuery = filesQuery.Where(f => f.Tags != null && f.Tags.Contains(tag));
        }

        if (!string.IsNullOrEmpty(contentType))
        {
            filesQuery = filesQuery.Where(f => f.ContentType.StartsWith(contentType));
        }

        var files = await filesQuery.OrderByDescending(f => f.UploadedAt).ToListAsync();

        return Ok(files.Select(MapToDto));
    }

    [HttpGet("tags/{entityType}/{entityId}")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllTags(string entityType, int entityId)
    {
        var files = await _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId && f.Tags != null)
            .Select(f => f.Tags)
            .ToListAsync();

        var allTags = new HashSet<string>();
        foreach (var tagsJson in files)
        {
            if (!string.IsNullOrEmpty(tagsJson))
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<string[]>(tagsJson);
                    if (tags != null)
                    {
                        foreach (var tag in tags)
                        {
                            allTags.Add(tag);
                        }
                    }
                }
                catch { }
            }
        }

        return Ok(allTags.OrderBy(t => t));
    }

    private async Task<(bool hasQuota, long usedBytes, long quotaBytes)> CheckQuotaAsync(
        string entityType, 
        int entityId, 
        long additionalBytes)
    {
        var files = await _context.EntityFiles
            .Where(f => f.EntityType == entityType && f.EntityId == entityId)
            .ToListAsync();

        var usedBytes = files.Sum(f => f.FileSizeBytes);
        var hasQuota = (usedBytes + additionalBytes) <= DefaultQuotaBytes;

        return (hasQuota, usedBytes, DefaultQuotaBytes);
    }

    private static EntityFileDto MapToDto(EntityFile file)
    {
        string[]? tags = null;
        if (!string.IsNullOrEmpty(file.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<string[]>(file.Tags);
            }
            catch { }
        }

        return new EntityFileDto(
            file.Id,
            file.EntityType,
            file.EntityId,
            file.FileName,
            file.OriginalFileName,
            file.FileSizeBytes,
            file.ContentType,
            file.Description,
            tags,
            !string.IsNullOrEmpty(file.ThumbnailPath),
            file.UploadedAt,
            file.UploadedBy,
            file.LastAccessedAt,
            file.AccessCount,
            file.RetentionUntil,
            file.IsCompressed,
            file.OriginalSizeBytes
        );
    }

    public record BatchMetadataUpdateRequest(List<int> FileIds, string? Description, string[]? Tags);
}
