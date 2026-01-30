using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace Ligot.Web;

// DTOs
public record UserDto(
    int Id,
    string WindowsUsername,
    string DisplayName,
    string? Email,
    bool IsActive,
    List<RoleDto>? Roles);

public record UserCreateDto(
    [Required] string WindowsUsername,
    [Required] string DisplayName,
    string? Email,
    bool IsActive);

/// <summary>
/// Role DTO with counts. UserCount and CoverageCount represent the number of users and customers
/// assigned to this role. CoverageCount of 0 means the role has access to all customers.
/// </summary>
public record RoleDto(
    int Id,
    string Name,
    string? Description,
    string PagePermissions,
    int UserCount = 0,
    int CoverageCount = 0);

public record UsersByRoleDto(
    int Id,
    string WindowsUsername,
    string DisplayName,
    string? Email,
    bool IsActive);

public record RoleCreateDto(
    [Required] string Name,
    string? Description,
    [Required] string PagePermissions);

public record CustomersByRoleDto(
    int Id,
    string Name,
    string? ContactPerson,
    string? Email,
    string? Phone);

public class AdminApiClient
{
    private readonly HttpClient _http;

    public AdminApiClient(HttpClient http)
    {
        _http = http;
    }

    // User operations
    public async Task<UserDto[]> GetUsersAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto[]>("/api/admin/users", ct) 
                   ?? Array.Empty<UserDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<UserDto>();
        }
    }

    public async Task<UserDto?> GetUserAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>($"/api/admin/users/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>($"/api/admin/users/by-username/{Uri.EscapeDataString(username)}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<UserDto?> CreateUserAsync(UserCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("/api/admin/users", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<UserDto>(ct);
        
        // Read error details
        var errorContent = await res.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException($"Failed to create user: {res.StatusCode} - {errorContent}");
    }

    public async Task<bool> UpdateUserAsync(int id, UserDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/admin/users/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/admin/users/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // Role assignment
    public async Task<bool> AssignRoleAsync(int userId, int roleId, CancellationToken ct = default)
    {
        var res = await _http.PostAsync($"/api/admin/users/{userId}/roles/{roleId}", null, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/admin/users/{userId}/roles/{roleId}", ct);
        return res.IsSuccessStatusCode;
    }

    // Role operations
    public async Task<RoleDto[]> GetRolesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<RoleDto[]>("/api/admin/roles", ct) 
                   ?? Array.Empty<RoleDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<RoleDto>();
        }
    }

    public async Task<RoleDto?> GetRoleAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<RoleDto>($"/api/admin/roles/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<RoleDto?> CreateRoleAsync(RoleCreateDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("/api/admin/roles", dto, ct);
        if (res.IsSuccessStatusCode)
            return await res.Content.ReadFromJsonAsync<RoleDto>(ct);
        
        // Read error details
        var errorContent = await res.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException($"Failed to create role: {res.StatusCode} - {errorContent}");
    }

    public async Task<bool> UpdateRoleAsync(int id, RoleDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/admin/roles/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRoleAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/admin/roles/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // Get users by role
    public async Task<UsersByRoleDto[]> GetUsersByRoleAsync(int roleId, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<UsersByRoleDto[]>($"/api/admin/roles/{roleId}/users", ct)
                   ?? Array.Empty<UsersByRoleDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<UsersByRoleDto>();
        }
    }

    // Coverage operations
    public async Task<CustomersByRoleDto[]> GetCustomersByRoleAsync(int roleId, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<CustomersByRoleDto[]>($"/api/admin/roles/{roleId}/coverage", ct)
                   ?? Array.Empty<CustomersByRoleDto>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<CustomersByRoleDto>();
        }
    }

    public async Task<bool> AssignCoverageAsync(int roleId, int customerId, CancellationToken ct = default)
    {
        var res = await _http.PostAsync($"/api/admin/roles/{roleId}/coverage/{customerId}", null, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveCoverageAsync(int roleId, int customerId, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"/api/admin/roles/{roleId}/coverage/{customerId}", ct);
        return res.IsSuccessStatusCode;
    }
}

