

using System.Collections.Generic;

namespace StudentPortal.Web.Models
{
    public class QuizSubmissionViewModel
    {
        public int QuizId { get; set; }
        public string UserId { get; set; }
        public List<UserAnswerViewModel> UserAnswers { get; set; }
    }

    public class UserAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public string Confidence { get; set; }
    }
}
