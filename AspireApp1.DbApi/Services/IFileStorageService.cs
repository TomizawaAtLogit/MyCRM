namespace AspireApp1.DbApi.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string entityType, int entityId);
    Task<(Stream stream, string contentType)> GetFileAsync(string storagePath);
    Task DeleteFileAsync(string storagePath);
    Task<string?> GenerateThumbnailAsync(string storagePath, string contentType);
    Task<(Stream stream, long compressedSize)> CompressFileAsync(Stream fileStream, string fileName);
    string SanitizeFileName(string fileName);
}
