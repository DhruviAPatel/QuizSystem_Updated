namespace StudentPortal.Web.Models.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public Quiz Quiz { get; set; }
        public string QuestionType { get; set; }
        public double? MinAcceptableValue { get; set; } 
        public double? MaxAcceptableValue { get; set; } 
    }
}
