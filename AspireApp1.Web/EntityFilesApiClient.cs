using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace AspireApp1.Web;

// DTOs matching backend
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

public class EntityFilesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EntityFilesApiClient> _logger;

    public EntityFilesApiClient(HttpClient httpClient, ILogger<EntityFilesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EntityFileDto[]?> GetFilesAsync(string entityType, int entityId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EntityFileDto[]>($"api/EntityFiles/{entityType}/{entityId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get files for {EntityType} {EntityId}", entityType, entityId);
            return null;
        }
    }

    public async Task<EntityFileDto?> GetFileAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EntityFileDto>($"api/EntityFiles/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file {FileId}", id);
            return null;
        }
    }

    public async Task<EntityFileDto?> UploadFileAsync(
        string entityType,
        int entityId,
        IBrowserFile file,
        string? description = null,
        string[]? tags = null,
        long maxFileSize = 100 * 1024 * 1024,
        IProgress<double>? progress = null)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            
            // Add file
            var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
            var contentType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType;
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", file.Name);
            
            // Add metadata
            content.Add(new StringContent(entityType), "entityType");
            content.Add(new StringContent(entityId.ToString()), "entityId");
            
            if (!string.IsNullOrEmpty(description))
            {
                content.Add(new StringContent(description), "description");
            }
            
            if (tags != null && tags.Length > 0)
            {
                content.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(tags)), "tags");
            }

            var response = await _httpClient.PostAsync("api/EntityFiles/upload", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<EntityFileDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", file.Name);
            return null;
        }
    }

    public async Task<EntityFileDto[]?> UploadBatchAsync(
        string entityType,
        int entityId,
        IReadOnlyList<IBrowserFile> files,
        string? description = null,
        long maxFileSize = 100 * 1024 * 1024)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            
            // Add metadata
            content.Add(new StringContent(entityType), "entityType");
            content.Add(new StringContent(entityId.ToString()), "entityId");
            
            if (!string.IsNullOrEmpty(description))
            {
                content.Add(new StringContent(description), "description");
            }
            
            // Add files
            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "files", file.Name);
            }

            var response = await _httpClient.PostAsync("api/EntityFiles/upload-batch", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<EntityFileDto[]>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload batch files");
            return null;
        }
    }

    public async Task<(byte[] data, string contentType, string fileName)?> DownloadFileAsync(int fileId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/EntityFiles/download/{fileId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"file_{fileId}";

            return (data, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FileId}", fileId);
            return null;
        }
    }

    public string GetDownloadUrl(int fileId)
    {
        // Return the full API URL for the HttpClient base address
        return $"{_httpClient.BaseAddress}api/EntityFiles/download/{fileId}";
    }

    public string GetDownloadAllUrl(string entityType, int entityId)
    {
        // Return the full API URL for the HttpClient base address
        return $"{_httpClient.BaseAddress}api/EntityFiles/download-all/{entityType}/{entityId}";
    }

    public string GetThumbnailUrl(int fileId)
    {
        // Use relative URL for browser navigation
        return $"/api/EntityFiles/thumbnail/{fileId}";
    }

    public async Task<bool> UpdateFileMetadataAsync(int id, EntityFileUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/EntityFiles/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update file metadata {FileId}", id);
            return false;
        }
    }

    public async Task<bool> BatchUpdateMetadataAsync(List<int> fileIds, string? description, string[]? tags)
    {
        try
        {
            var request = new { FileIds = fileIds, Description = description, Tags = tags };
            var response = await _httpClient.PutAsJsonAsync("api/EntityFiles/batch-update-metadata", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to batch update metadata");
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/EntityFiles/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileId}", id);
            return false;
        }
    }

    public async Task<FileQuotaDto?> GetQuotaAsync(string entityType, int entityId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<FileQuotaDto>($"api/EntityFiles/quota/{entityType}/{entityId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get quota for {EntityType} {EntityId}", entityType, entityId);
            return null;
        }
    }

    public async Task<EntityFileDto[]?> SearchFilesAsync(
        string entityType,
        int entityId,
        string? query = null,
        string? tag = null,
        string? contentType = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(query)) queryParams.Add($"query={Uri.EscapeDataString(query)}");
            if (!string.IsNullOrEmpty(tag)) queryParams.Add($"tag={Uri.EscapeDataString(tag)}");
            if (!string.IsNullOrEmpty(contentType)) queryParams.Add($"contentType={Uri.EscapeDataString(contentType)}");
            
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"api/EntityFiles/search/{entityType}/{entityId}{queryString}";
            
            return await _httpClient.GetFromJsonAsync<EntityFileDto[]>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search files");
            return null;
        }
    }

    public async Task<string[]?> GetAllTagsAsync(string entityType, int entityId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<string[]>($"api/EntityFiles/tags/{entityType}/{entityId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tags for {EntityType} {EntityId}", entityType, entityId);
            return null;
        }
    }
}
