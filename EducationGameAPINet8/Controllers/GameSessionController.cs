using EducationGameAPINet8.Models;
using EducationGameAPINet8.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EducationGameAPINet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionController(IGameSessionService gameSessionService) : Controller
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateGameSession([FromBody] GameSessionCreateDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }

                var sessionId = await gameSessionService.CreateGameSessionAsync(dto, userId);
                return Ok(new { sessionId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("latest-session")]
        public async Task<IActionResult> GetLatestSession([FromQuery] string gameType)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                var latestSession = await gameSessionService.GetLatestSessionAsync(userId, gameType);
                if (latestSession == null)
                {
                    return NotFound("No game session found for this user.");
                }
                return Ok(latestSession);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetGameSummary([FromQuery] string gameType)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                var summary = await gameSessionService.GetGameSummaryAsync(userId, gameType);
                if (summary == null)
                {
                    return NotFound("No game sessions found for this user.");
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("unlocked-status")]
        public async Task<IActionResult> GetUnlockedGameStatus()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                var unlockedStatus = await gameSessionService.GetUnlockStatusWithScoresAsync(userId);
                return Ok(unlockedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
