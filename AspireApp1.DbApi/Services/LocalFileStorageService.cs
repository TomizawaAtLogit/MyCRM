using System.IO.Compression;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AspireApp1.DbApi.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;
    private readonly ILogger<LocalFileStorageService> _logger;
    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100MB
    private const long CompressionThreshold = 5 * 1024 * 1024; // 5MB

    private static readonly HashSet<string> AllowedContentTypes = new()
    {
        // Images
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp",
        // Documents
        "application/pdf",
        "application/msword", // .doc
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
        "application/vnd.ms-excel", // .xls
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
        "application/vnd.ms-powerpoint", // .ppt
        "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .pptx
        // Text
        "text/plain", "text/csv", "text/html", "text/xml",
        "application/json", "application/xml",
        // Archives
        "application/zip", "application/x-zip-compressed",
        "application/x-rar-compressed", "application/x-7z-compressed",
        // Other
        "application/octet-stream"
    };

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _baseStoragePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "FileUploads");
        
        // Ensure base directory exists
        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
        }
    }

    public string SanitizeFileName(string fileName)
    {
        // Remove any path components
        fileName = Path.GetFileName(fileName);
        
        // Replace invalid file system characters with underscore
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        
        // Also replace some potentially problematic characters
        fileName = fileName.Replace(':', '_').Replace('*', '_').Replace('?', '_')
                          .Replace('"', '_').Replace('<', '_').Replace('>', '_')
                          .Replace('|', '_');
        
        // Limit length to 255 characters
        if (fileName.Length > 255)
        {
            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            fileName = nameWithoutExtension.Substring(0, 255 - extension.Length) + extension;
        }
        
        return fileName;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string entityType, int entityId)
    {
        fileName = SanitizeFileName(fileName);
        
        // Create entity-specific directory
        var entityDirectory = Path.Combine(_baseStoragePath, entityType, entityId.ToString());
        if (!Directory.Exists(entityDirectory))
        {
            Directory.CreateDirectory(entityDirectory);
        }
        
        // Generate unique file name to avoid conflicts
        var fileExtension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(entityDirectory, uniqueFileName);
        
        // Save file
        using (var fileStreamOutput = File.Create(filePath))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }
        
        // Return relative path with forward slashes for cross-platform compatibility
        var relativePath = Path.Combine(entityType, entityId.ToString(), uniqueFileName);
        return relativePath.Replace('\\', '/');
    }

    public async Task<(Stream stream, string contentType)> GetFileAsync(string storagePath)
    {
        // Normalize path separators for the current platform
        storagePath = storagePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_baseStoragePath, storagePath);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", storagePath);
        }
        
        var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var contentType = GetContentType(fullPath);
        
        return (fileStream, contentType);
    }

    public Task DeleteFileAsync(string storagePath)
    {
        // Normalize path separators for the current platform
        storagePath = storagePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_baseStoragePath, storagePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file: {FilePath}", storagePath);
        }
        
        return Task.CompletedTask;
    }

    public async Task<string?> GenerateThumbnailAsync(string storagePath, string contentType)
    {
        if (!contentType.StartsWith("image/"))
        {
            return null;
        }
        
        // Normalize path separators for the current platform
        storagePath = storagePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_baseStoragePath, storagePath);
        if (!File.Exists(fullPath))
        {
            return null;
        }
        
        try
        {
            var thumbnailDirectory = Path.Combine(Path.GetDirectoryName(fullPath)!, "thumbnails");
            if (!Directory.Exists(thumbnailDirectory))
            {
                Directory.CreateDirectory(thumbnailDirectory);
            }
            
            var thumbnailFileName = $"thumb_{Path.GetFileNameWithoutExtension(fullPath)}.jpg";
            var thumbnailPath = Path.Combine(thumbnailDirectory, thumbnailFileName);
            
            using (var image = await Image.LoadAsync(fullPath))
            {
                // Resize to max 200x200 maintaining aspect ratio
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                }));
                
                await image.SaveAsJpegAsync(thumbnailPath, new JpegEncoder { Quality = 75 });
            }
            
            // Return relative path with forward slashes for cross-platform compatibility
            var relativeThumbnailPath = Path.Combine(
                Path.GetDirectoryName(storagePath)!,
                "thumbnails",
                thumbnailFileName
            );
            
            return relativeThumbnailPath.Replace('\\', '/');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate thumbnail for {FilePath}", storagePath);
            return null;
        }
    }

    public async Task<(Stream stream, long compressedSize)> CompressFileAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        
        // Don't compress already compressed files
        var skipCompression = new[] { ".zip", ".rar", ".7z", ".gz", ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".mp4" };
        if (skipCompression.Contains(extension))
        {
            fileStream.Position = 0;
            return (fileStream, fileStream.Length);
        }
        
        var memoryStream = new MemoryStream();
        
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
        {
            await fileStream.CopyToAsync(gzipStream);
        }
        
        memoryStream.Position = 0;
        return (memoryStream, memoryStream.Length);
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".html" => "text/html",
            ".xml" => "text/xml",
            ".json" => "application/json",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    public static bool IsContentTypeAllowed(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return false;
        }
        return AllowedContentTypes.Contains(contentType.ToLower());
    }
}
