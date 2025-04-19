//using StudentPortal.Web.Models;

//namespace StudentPortal.Web.Services
//{
//    public interface IQuizService
//    {
//        QuizViewModel GetQuizById(int quizId); // Retrieves the quiz details by ID
//        QuizResultViewModel EvaluateQuiz(QuizSubmissionViewModel submission); // Evaluates the quiz submission and returns the result
//    }
//}

using StudentPortal.Web.Models;

namespace StudentPortal.Web.Services
{

    public interface IQuizService
    {
        QuizViewModel GetQuizById(int quizId);
        int EvaluateQuiz(int quizId, Dictionary<int, string> userAnswers);

        void SaveQuizResult(int quizId, int userId, int score);
        List<QuizResultViewModel> GetQuizResults(int quizId);


        //int EvaluateQuiz(QuizSubmissionViewModel submission);

        //object EvaluateQuiz(QuizSubmissionViewModel submission);
        //int EvaluateQuiz(QuizSubmissionViewModel submission);


    }
}
