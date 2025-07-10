using EducationGameAPINet8.Models;

namespace EducationGameAPINet8.Services
{
    public interface IGameSessionService
    {
        Task<Guid> CreateGameSessionAsync(GameSessionCreateDto gameSessionCreateDto, Guid userId);
        Task<GameSessionDto?> GetLatestSessionAsync(Guid userId, string gameType);
        Task<GameSummaryDto> GetGameSummaryAsync(Guid userId, string gameType);
        Task<List<(string GameType, double TotalScore)>> GetGameScoresAsync(Guid userId);
        Task<List<string>> GetUnlockedGamesAsync(Guid userId);
        Task<List<GameUnlockStatusDto>> GetUnlockStatusWithScoresAsync(Guid userId);
    }
}
