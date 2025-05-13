

namespace StudentPortal.Web.Models
{
    public class QuizResultViewModel
    {
        public string QuizTitle { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double TotalScore { get; set; }
        public int PercentageScore { get; set; } 
        public int StudentId { get; set; }
        public DateTime TakenDate { get; set; }
    }
}

