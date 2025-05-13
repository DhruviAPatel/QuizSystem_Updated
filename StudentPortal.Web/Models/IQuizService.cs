
using StudentPortal.Web.Models;

namespace StudentPortal.Web.Services
{

    public interface IQuizService
    {
        QuizViewModel GetQuizById(int quizId);
       (double Score, int CorrectCount) EvaluateQuiz(int quizId, Dictionary<int, (string Answer, string Confidence)> userAnswers);

        void SaveQuizResult(int quizId, int userId, double TotalScore, int PercentageScore);
        List<QuizResultViewModel> GetQuizResults(int quizId);


    }
}
