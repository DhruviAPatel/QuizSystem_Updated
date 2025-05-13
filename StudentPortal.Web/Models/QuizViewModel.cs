

using System.Collections.Generic;

namespace StudentPortal.Web.Models
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        
        public int TotalQuestions { get; set; }

    }
}



