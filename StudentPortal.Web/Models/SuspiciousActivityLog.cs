namespace StudentPortal.Web.Models
{
    public class SuspiciousActivityLog
    {
        public int QuizId { get; set; }
        public string UserId { get; set; }
        public string EventType { get; set; }
        public int? QuestionId { get; set; }
        public double? TimeTaken { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
