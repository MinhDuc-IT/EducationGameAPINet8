using EducationGameAPINet8.Data;
using EducationGameAPINet8.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationGameAPINet8.Services
{
    public class GameSessionService(AppDbContext context, IConfiguration configuration) : IGameSessionService
    {
        public async Task<Guid> CreateGameSessionAsync(GameSessionCreateDto gameSessionCreateDto, Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Tổng số câu có đáp án đúng (lần 1 + lần 2)
            int totalCorrect = gameSessionCreateDto.CorrectFirstTry + gameSessionCreateDto.CorrectSecondTry;

            // Tổng số lựa chọn của người dùng
            int totalAttempts = totalCorrect + gameSessionCreateDto.TotalWrongAnswers;

            double totalAccuracy = 0;
            if (totalAttempts > 0)
            {
                totalAccuracy = (double)totalCorrect / totalAttempts;
            }

            double totalScore = (gameSessionCreateDto.CorrectFirstTry * 2) + (gameSessionCreateDto.CorrectSecondTry * 1);

            var gameSession = new Entities.GameSession
            {
                UserId = userId,
                GameType = gameSessionCreateDto.GameType,
                StartTime = gameSessionCreateDto.StartTime,
                EndTime = gameSessionCreateDto.EndTime,
                MaxRounds = gameSessionCreateDto.MaxRounds,
                CorrectFirstTry = gameSessionCreateDto.CorrectFirstTry,
                CorrectSecondTry = gameSessionCreateDto.CorrectSecondTry,
                TotalWrongAnswers = gameSessionCreateDto.TotalWrongAnswers,
                Accuracy = totalAccuracy,
                Score = totalScore
            };

            context.GameSessions.Add(gameSession);
            await context.SaveChangesAsync();

            return gameSession.Id;
        }

        public async Task<GameSummaryDto?> GetGameSummaryAsync(Guid userId, string gameType)
        {
            var gameSessions = await context.GameSessions
                .Where(gs => gs.UserId == userId && gs.GameType == gameType)
                .ToListAsync();

            if (gameSessions.Count == 0) return null;

            var totalSeconds = gameSessions.Sum(gs => (int)(gs.EndTime - gs.StartTime).TotalSeconds);
            var totalScore = gameSessions.Sum(gs => gs.Score);
            var averageAccuracy = gameSessions.Average(gs => gs.Accuracy);

            return new GameSummaryDto
            {
                TotalSeconds = totalSeconds,
                TotalScore = totalScore,
                Accuracy = averageAccuracy
            };
        }

        public async Task<GameSessionDto?> GetLatestSessionAsync(Guid userId, string gameType)
        {
            var latestSession = await context.GameSessions
                .Where(gs => gs.UserId == userId && gs.GameType == gameType)
                .OrderByDescending(gs => gs.EndTime)
                .FirstOrDefaultAsync();

            if (latestSession == null) return null;

            var seconds = (int)(latestSession.EndTime - latestSession.StartTime).TotalSeconds;

            return new GameSessionDto
            {
                Seconds = seconds,
                MaxRounds = latestSession.MaxRounds,
                CorrectFirstTry = latestSession.CorrectFirstTry,
                CorrectSecondTry = latestSession.CorrectSecondTry
            };
        }

        public async Task<List<(string GameType, double TotalScore)>> GetGameScoresAsync(Guid userId)
        {
            var sessions = await context.GameSessions
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.GameType)
                .Select(g => new
                {
                    GameType = g.Key,
                    TotalScore = g.Sum(s => s.Score)
                })
                .ToListAsync();

            return sessions.Select(s => (s.GameType, s.TotalScore)).ToList();
        }

        public async Task<List<string>> GetUnlockedGamesAsync(Guid userId)
        {
            var scores = await GetGameScoresAsync(userId);

            var gameOrder = new List<string> { "SheepCounting", "SheepColorCounting", "NewGame" };
            var unlocked = new List<string>();

            double prevScore = 0;

            for (int i = 0; i < gameOrder.Count; i++)
            {
                var game = gameOrder[i];
                var currentScore = scores.FirstOrDefault(s => s.GameType == game).TotalScore;

                bool isUnlocked = i == 0 || prevScore >= 50;

                if (isUnlocked)
                {
                    unlocked.Add(game);
                    prevScore = currentScore;
                }
            }

            return unlocked;
        }

        public async Task<List<GameUnlockStatusDto>> GetUnlockStatusWithScoresAsync(Guid userId)
        {
            var scores = await GetGameScoresAsync(userId);
            var gameOrder = new List<string> { "SheepCounting", "SheepColorCounting", "NewGame" };

            var result = new List<GameUnlockStatusDto>();
            double prevScore = 0;

            for (int i = 0; i < gameOrder.Count; i++)
            {
                var game = gameOrder[i];
                double score = scores.FirstOrDefault(s => s.GameType == game).TotalScore;

                bool unlocked = i == 0 || prevScore >= 50;

                if (unlocked)
                {
                    prevScore = score;
                }

                result.Add(new GameUnlockStatusDto
                {
                    GameType = game,
                    Unlocked = unlocked,
                    TotalScore = score
                });
            }

            return result;
        }
    }
}
