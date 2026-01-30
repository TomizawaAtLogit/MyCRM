using System.Net.Http.Json;

namespace Ligot.Web;

// DTO
public record UserPreferenceDto(string? PreferredLanguage);

public class UserPreferencesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserPreferencesApiClient> _logger;

    public UserPreferencesApiClient(HttpClient httpClient, ILogger<UserPreferencesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserPreferenceDto?> GetUserPreferencesAsync(string username)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserPreferenceDto>($"api/userpreferences/{Uri.EscapeDataString(username)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user preferences for {Username}", username);
            return null;
        }
    }

    public async Task<bool> UpdateUserPreferencesAsync(string username, UserPreferenceDto preferences)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/userpreferences/{Uri.EscapeDataString(username)}", preferences);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user preferences for {Username}", username);
            return false;
        }
    }
}

