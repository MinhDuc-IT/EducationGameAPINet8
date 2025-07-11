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
        public async Task<IActionResult> CreateGameSession([FromBody] GameSessionCreateDto dto) // hàm tạo mới và lưu thông tin lượt chơi user vừa chơi
        {
            try
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return BadRequest("User identifier is missing.");
                }

                var userId = Guid.Parse(nameIdentifier); // lấy userId từ claim trong token

                var sessionId = await gameSessionService.CreateGameSessionAsync(dto, userId); // gọi đến service xử lý
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
        public async Task<IActionResult> GetLatestSession([FromQuery] string gameType) // hàm lấy phiên chơi mới nhất của loại game cụ thể gameType
        {
            try
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return BadRequest("User identifier is missing.");
                }

                var userId = Guid.Parse(nameIdentifier); // lấy userId từ claim trong token
                
                var latestSession = await gameSessionService.GetLatestSessionAsync(userId, gameType); // gọi đến service xử lý
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
        public async Task<IActionResult> GetGameSummary([FromQuery] string gameType) // hàm lấy thông tin tổng hợp của loại game cụ thể gameType
        {
            try
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return BadRequest("User identifier is missing.");
                }

                var userId = Guid.Parse(nameIdentifier); // lấy userId từ claim trong token
                
                var summary = await gameSessionService.GetGameSummaryAsync(userId, gameType); // gọi đến service xử lý
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
        public async Task<IActionResult> GetUnlockedGameStatus() // hàm lấy trạng thái mở khóa của các game
        {
            try
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return BadRequest("User identifier is missing.");
                }

                var userId = Guid.Parse(nameIdentifier); // lấy userId từ claim trong token
                
                var unlockedStatus = await gameSessionService.GetUnlockStatusWithScoresAsync(userId); // gọi đến service xử lý
                return Ok(unlockedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
