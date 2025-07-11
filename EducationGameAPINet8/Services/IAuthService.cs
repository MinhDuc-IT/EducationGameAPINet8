using EducationGameAPINet8.Entities;
using EducationGameAPINet8.Models;

namespace EducationGameAPINet8.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request, Guid userId);
        Task LogoutAsync(Guid userId);
    }
}
