namespace EducationGameAPINet8.Entities
{
    public class GameSession
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // id lượt chơi game

        public Guid UserId { get; set; } // id của user
        public User User { get; set; }

        public string GameType { get; set; } // kiểu game, tên game

        public DateTime StartTime { get; set; } // thời gian bắt đầu chơi
        public DateTime EndTime { get; set; } // thời gian kết thúc lượt chơi

        public int MaxRounds { get; set; } // tổng số vòng mỗi lượt chơi
        public int CorrectFirstTry { get; set; } // số lần đúng ngay từ lần chọn đầu tiên
        public int CorrectSecondTry { get; set; } // số lần đúng tại lần chọn thứ hai
        public int TotalWrongAnswers { get; set; } // tổng số lần chọn sai

        public double Accuracy { get; set; } // tỷ lệ chọn đúng
        public double Score { get; set; } // tổng điểm
    }
}
