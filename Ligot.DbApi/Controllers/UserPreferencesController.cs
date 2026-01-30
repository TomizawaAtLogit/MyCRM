using Ligot.DbApi.DTOs;
using Ligot.DbApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ligot.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserPreferencesController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly ILogger<UserPreferencesController> _logger;

    public UserPreferencesController(IUserRepository userRepo, ILogger<UserPreferencesController> logger)
    {
        _userRepo = userRepo;
        _logger = logger;
    }

    /// <summary>
    /// Get user preferences by username
    /// </summary>
    [HttpGet("{username}")]
    public async Task<ActionResult<UserPreferenceDto>> GetUserPreferences(string username)
    {
        try
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new UserPreferenceDto
            {
                PreferredLanguage = user.PreferredLanguage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preferences for user {Username}", username);
            return StatusCode(500, new { message = "An error occurred while retrieving user preferences" });
        }
    }

    /// <summary>
    /// Update user preferences by username
    /// </summary>
    [HttpPut("{username}")]
    public async Task<ActionResult<UserPreferenceDto>> UpdateUserPreferences(string username, [FromBody] UserPreferenceDto preferences)
    {
        try
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.PreferredLanguage = preferences.PreferredLanguage;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);

            return Ok(new UserPreferenceDto
            {
                PreferredLanguage = user.PreferredLanguage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {Username}", username);
            return StatusCode(500, new { message = "An error occurred while updating user preferences" });
        }
    }
}

