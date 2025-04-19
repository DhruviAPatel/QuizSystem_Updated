
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

        //public int EvaluateQuiz(int quizId, Dictionary<int, string> userAnswers)
        //{
        //    var correctAnswers = _context.Questions
        //        .Where(q => q.QuizId == quizId)
        //        .ToDictionary(q => q.QuestionId, q => q.CorrectOption);

        //    int score = userAnswers.Count(answer =>
        //        correctAnswers.ContainsKey(answer.Key) &&
        //        correctAnswers[answer.Key].Equals(answer.Value, StringComparison.OrdinalIgnoreCase));

        //    return score;
        //}

        public int EvaluateQuiz(int quizId, Dictionary<int, string> userAnswers)
        {
            var questions = _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToList();

            int score = 0;

            foreach (var question in questions)
            {
                if (!userAnswers.TryGetValue(question.QuestionId, out string userAnswer) || string.IsNullOrWhiteSpace(userAnswer))
                    continue;

                switch (question.QuestionType)
                {
                    case "MCQ":
                        if (!string.IsNullOrEmpty(question.CorrectOption) &&
                            question.CorrectOption.Equals(userAnswer, StringComparison.OrdinalIgnoreCase))
                        {
                            score++;
                        }
                        break;

                    case "Numerical":
                        if (double.TryParse(userAnswer, out double userValue) &&
                            question.MinAcceptableValue.HasValue &&
                            question.MaxAcceptableValue.HasValue)
                        {
                            if (userValue >= question.MinAcceptableValue.Value &&
                                userValue <= question.MaxAcceptableValue.Value)
                            {
                                score++;
                            }
                        }
                        break;

                    case "Subjective":
                        string normalizedUserAnswer = NormalizeText(userAnswer);
                        string normalizedCorrectAnswer = NormalizeText(question.CorrectOption);

                        // Option 1: Exact normalized match
                        if (normalizedUserAnswer == normalizedCorrectAnswer)
                        {
                            score++;
                        }

                        // Option 2: Partial keyword-based match (if full match fails)
                        else
                        {
                            var keywords = normalizedCorrectAnswer.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            int matchedKeywords = keywords.Count(k => normalizedUserAnswer.Contains(k));
                            double matchPercentage = (double)matchedKeywords / keywords.Length;

                            if (matchPercentage >= 0.7) // 70% keywords matched
                            {
                                score++;
                            }
                        }
                        break;
                }
            }

            return score;
        }

        // Helper function to normalize string
        private string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Lowercase + trim + remove extra spaces
            return string.Join(" ", input
                .ToLower()
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }


        public void SaveQuizResult(int quizId, int userId, int score)
        {
            var quizResult = new QuizResult
            {
                QuizId = quizId,
                UserId = userId,
                Score = score,
                TakenDate = DateTime.UtcNow
            };

            _context.QuizResults.Add(quizResult); // Assuming _context is your ApplicationDbContext
            _context.SaveChanges();
        }

        public List<QuizResultViewModel> GetQuizResults(int quizId)
        {
            return _context.QuizResults
                .Where(qr => qr.QuizId == quizId)
                .Select(qr => new QuizResultViewModel
                {
                    StudentId = qr.UserId,
                    Score = qr.Score,
                    TakenDate = qr.TakenDate
                })
                .ToList();
        }

        
    }
}

