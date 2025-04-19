

namespace StudentPortal.Web.Models
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public string QuestionType { get; set; } 
        public double? MinAcceptableValue { get; set; } // For numerical
        public double? MaxAcceptableValue { get; set; } // For numerical
    }
}


