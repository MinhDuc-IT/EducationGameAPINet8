namespace EducationGameAPINet8.Models
{
    public class GameSessionDto
    {
        public int Seconds { get; set; }
        public int MaxRounds { get; set; }
        public int CorrectFirstTry { get; set; }
        public int CorrectSecondTry { get; set; }
    }
}
