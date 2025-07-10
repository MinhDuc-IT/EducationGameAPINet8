namespace EducationGameAPINet8.Entities
{
    public class GameSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string GameType { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int MaxRounds { get; set; }
        public int CorrectFirstTry { get; set; }
        public int CorrectSecondTry { get; set; }
        public int TotalWrongAnswers { get; set; }

        public double Accuracy { get; set; }
        public double Score { get; set; }
    }
}
