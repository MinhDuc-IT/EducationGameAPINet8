using EducationGameAPINet8.Entities;
using EducationGameAPINet8.Models;
using EducationGameAPINet8.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EducationGameAPINet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : Controller
    {
        public static User user = new();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request) // hàm đăng ký tài khoản mới
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists.");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request) // hàm đăng nhập
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request) // hàm tạo mới token
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(nameIdentifier))
            {
                return BadRequest("User identifier is missing.");
            }

            var userId = Guid.Parse(nameIdentifier);
            var result = await authService.RefreshTokenAsync(request, userId);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token or access token expired.");
            }
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() // hàm đăng xuất
        {
            try
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return BadRequest("User identifier is missing.");
                }

                var userId = Guid.Parse(nameIdentifier);
                await authService.LogoutAsync(userId);
                return Ok("Logged out");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}
