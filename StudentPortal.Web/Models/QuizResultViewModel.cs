//namespace StudentPortal.Web.Models
//{
//    public class QuizResultViewModel
//    {
//        public int Score { get; set; }
//    }
//}



namespace StudentPortal.Web.Models
{
    public class QuizResultViewModel
    {
        public string QuizTitle { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int Score { get; set; } // Percentage score
        public int StudentId { get; set; }
        public DateTime TakenDate { get; set; }
    }
}

