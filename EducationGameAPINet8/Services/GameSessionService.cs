using EducationGameAPINet8.Data;
using EducationGameAPINet8.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationGameAPINet8.Services
{
    public class GameSessionService(AppDbContext context) : IGameSessionService
    {
        public async Task<Guid> CreateGameSessionAsync(GameSessionCreateDto gameSessionCreateDto, Guid userId) // hàm tạo và lưu thông tin lượt chơi game mà user vừa chơi xong
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

            double totalAccuracy = 0; // biến lưu tỷ lệ chọn đúng
            if (totalAttempts > 0)
            {
                totalAccuracy = (double)totalCorrect / totalAttempts;
            }

            double totalScore = 0;

            if (gameSessionCreateDto.GameType == "SheepMemoryMatch")
            {
                totalScore = (gameSessionCreateDto.CorrectFirstTry * 2) + (gameSessionCreateDto.CorrectSecondTry * 1);
            }
            else
            {
                totalScore = (gameSessionCreateDto.CorrectFirstTry * 4) + (gameSessionCreateDto.CorrectSecondTry * 2);
            }

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

        public async Task<GameSummaryDto?> GetGameSummaryAsync(Guid userId, string gameType) // hàm lấy thông tin tổng hợp mà user đã chơi game này
        {
            var gameSessions = await context.GameSessions
                .Where(gs => gs.UserId == userId && gs.GameType == gameType)
                .ToListAsync();

            if (gameSessions.Count == 0) return null;

            var totalSeconds = gameSessions.Sum(gs => (int)(gs.EndTime - gs.StartTime).TotalSeconds); // tổng thời gian đã chơi game này (tính theo giây)
            var totalScore = gameSessions.Sum(gs => gs.Score); // tổng điểm đạt được của tất cả lượt chơi game này
            var averageAccuracy = gameSessions.Average(gs => gs.Accuracy); // trung bình tỷ lệ chọn đúng

            return new GameSummaryDto
            {
                TotalSeconds = totalSeconds,
                TotalScore = totalScore,
                Accuracy = averageAccuracy
            };
        }

        public async Task<GameSessionDto?> GetLatestSessionAsync(Guid userId, string gameType)  // hàm lấy thông tin lượt chơi mới nhất
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

        public async Task<List<(string GameType, double TotalScore)>> GetGameScoresAsync(Guid userId)  // hàm lấy tổng điểm của game
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

        public async Task<List<string>> GetUnlockedGamesAsync(Guid userId)  // hàm lấy các game được mở khóa cho user chơi
        {
            var scores = await GetGameScoresAsync(userId);

            var gameOrder = new List<string> { "SheepCounting", "SheepColorCounting", "SheepMemoryMatch", "SheepPatternRecognition", "FinalPoint" };
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

        public async Task<List<GameUnlockStatusDto>> GetUnlockStatusWithScoresAsync(Guid userId) // hàm lấy trạng thái mở khóa của game

        {
            var scores = await GetGameScoresAsync(userId);
            var gameOrder = new List<string> { "SheepCounting", "SheepColorCounting", "SheepMemoryMatch", "SheepPatternRecognition", "FinalPoint" };

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
