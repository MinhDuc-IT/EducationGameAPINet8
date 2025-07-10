namespace EducationGameAPINet8.Models
{
    public class GameSessionCreateDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GameType { get; set; }
        public int MaxRounds { get; set; }
        public int CorrectFirstTry { get; set; }
        public int CorrectSecondTry { get; set; }
        public int TotalWrongAnswers { get; set; }
    }
}
