//using System.Collections.Generic;

//namespace StudentPortal.Web.Models
//{
//    public class QuizSubmissionViewModel
//    {
//        public int QuizId { get; set; }
//        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>(); // QuestionId -> Selected Option (A, B, C, D)
//    }
//}

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
    }
}
