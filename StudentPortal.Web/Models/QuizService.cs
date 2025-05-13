
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;

        public QuizService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public QuizViewModel GetQuizById(int quizId)
        {
            var quiz = _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.QuizId == quizId);

            if (quiz == null) return null;

            return new QuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                TotalQuestions = quiz.TotalQuestions,
                Questions = quiz.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    QuizId = q.QuizId,
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption,
                    QuestionType = q.QuestionType,
                    MinAcceptableValue = q.MinAcceptableValue,
                    MaxAcceptableValue = q.MaxAcceptableValue
                }).ToList()
            };
        }
        public (double Score, int CorrectCount) EvaluateQuiz(int quizId, Dictionary<int, (string Answer, string Confidence)> userAnswers)
        {
            var questions = _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToList();

            double score = 0;
            int correctAnswers = 0;

            foreach (var question in questions)
            {
                if (!userAnswers.TryGetValue(question.QuestionId, out var userResponse) || string.IsNullOrWhiteSpace(userResponse.Answer))
                    continue;

                bool isCorrect = false;

                switch (question.QuestionType)
                {
                    case "MCQ":
                        isCorrect = !string.IsNullOrEmpty(question.CorrectOption) &&
                                    question.CorrectOption.Equals(userResponse.Answer, StringComparison.OrdinalIgnoreCase);
                        break;

                    case "Numerical":
                        if (double.TryParse(userResponse.Answer, out double userValue) &&
                            question.MinAcceptableValue.HasValue &&
                            question.MaxAcceptableValue.HasValue)
                        {
                            isCorrect = userValue >= question.MinAcceptableValue.Value &&
                                        userValue <= question.MaxAcceptableValue.Value;
                        }
                        break;

                    case "Subjective":
                        string normalizedUserAnswer = NormalizeText(userResponse.Answer);
                        string normalizedCorrectAnswer = NormalizeText(question.CorrectOption);

                        if (normalizedUserAnswer == normalizedCorrectAnswer)
                        {
                            isCorrect = true;
                        }
                        else
                        {
                            var keywords = normalizedCorrectAnswer.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            int matchedKeywords = keywords.Count(k => normalizedUserAnswer.Contains(k));
                            double matchPercentage = (double)matchedKeywords / keywords.Length;

                            if (matchPercentage >= 0.7)
                            {
                                isCorrect = true;
                            }
                        }
                        break;
                }

                double scoreChange = 0;
                string confidence = userResponse.Confidence?.ToLower() ?? "low";

                if (isCorrect)
                {
                    correctAnswers++;
                    score++;
                    scoreChange = confidence switch
                    {
                        "high" => 1.0,
                        "medium" => 0.7,
                        "low" => 0.5,
                        _ => 0.5
                    };
                }
                else
                {
                    scoreChange = confidence switch
                    {
                        "high" => -0.5,
                        "medium" => -0.3,
                        "low" => -0.1,
                        _ => 0
                    };
                }
                
                score += scoreChange;

                if (score < 0)
                    score = 0;
            }

            return (score, correctAnswers);
        }


        private string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            return string.Join(" ", input
                .ToLower()
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }


        public void SaveQuizResult(int quizId, int userId, double TotalScore,int PercentageScore)
        {
            var quizResult = new QuizResult
            {
                QuizId = quizId,
                UserId = userId,
                TotalScore = TotalScore,
                PercentageScore=PercentageScore,
                TakenDate = DateTime.UtcNow
            };

            _context.QuizResults.Add(quizResult); 
            _context.SaveChanges();
        }

        public List<QuizResultViewModel> GetQuizResults(int quizId)
        {
            return _context.QuizResults
                .Where(qr => qr.QuizId == quizId)
                .Select(qr => new QuizResultViewModel
                {
                    StudentId = qr.UserId,
                    TotalScore = qr.TotalScore,
                    PercentageScore = qr.PercentageScore,
                    TakenDate = qr.TakenDate
                })
                .ToList();
        }

        
    }
}

