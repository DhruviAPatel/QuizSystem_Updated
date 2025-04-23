
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Services;
using System.Collections.Generic;

namespace StudentPortal.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly string _connectionString =
            "Server=DESKTOP-1RV9DS2\\TEW_SQLEXPRESS;Database=StudentPortalDb;Trusted_Connection=True;TrustServerCertificate=True;";

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult QuizListForTeacher()
        {
            var quizzes = GetAllQuizzesFromDatabase();
            return View(quizzes);
        }
        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult QuizListForStudent()
        {
            var quizzes = GetAllQuizzesFromDatabase();
            return View(quizzes);
        }
        private List<QuizViewModel> GetAllQuizzesFromDatabase()
        {
            var quizzes = new List<QuizViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT QuizId, Title, Description FROM Quizzes";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        quizzes.Add(new QuizViewModel
                        {
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString()
                        });
                    }
                }
            }

            return quizzes;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditQuiz(int quizId)
        {
            var quiz = GetQuizWithQuestionsById(quizId);
            if (quiz != null)
            {
                return View("EditQuiz", quiz);
            }
            return RedirectToAction("QuizListForTeacher");
        }

        private QuizViewModel GetQuizWithQuestionsById(int quizId)
        {
            var quiz = new QuizViewModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var quizQuery = "SELECT * FROM Quizzes WHERE QuizId = @QuizId";
                using (var command = new SqlCommand(quizQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuizId", quizId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz.QuizId = (int)reader["QuizId"];
                            quiz.Title = reader["Title"].ToString();
                            quiz.Description = reader["Description"].ToString();
                        }
                    }
                }

                var questionQuery = "SELECT * FROM Questions WHERE QuizId = @QuizId";
                using (var command = new SqlCommand(questionQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuizId", quizId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quiz.Questions.Add(new QuestionViewModel
                            {
                                QuestionId = (int)reader["QuestionId"],
                                QuestionText = reader["QuestionText"].ToString(),
                                OptionA = reader["OptionA"]?.ToString(),
                                OptionB = reader["OptionB"]?.ToString(),
                                OptionC = reader["OptionC"]?.ToString(),
                                OptionD = reader["OptionD"]?.ToString(),
                                CorrectOption = reader["CorrectOption"] != DBNull.Value ? reader["CorrectOption"].ToString() : null,
                                QuestionType = reader["QuestionType"].ToString(),
                                MinAcceptableValue = reader["MinAcceptableValue"] != DBNull.Value ? (double?)Convert.ToDouble(reader["MinAcceptableValue"]) : null,
                                MaxAcceptableValue = reader["MaxAcceptableValue"] != DBNull.Value ? (double?)Convert.ToDouble(reader["MaxAcceptableValue"]) : null
                            });
                        }
                    }
                }
            }
            return quiz;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuiz(QuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Update Quiz Details
                    var query = "UPDATE Quizzes SET Title = @Title, Description = @Description WHERE QuizId = @QuizId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizId", model.QuizId);
                        command.Parameters.AddWithValue("@Title", model.Title);
                        command.Parameters.AddWithValue("@Description", model.Description);

                        command.ExecuteNonQuery();
                    }

                    // Update Questions
                    foreach (var question in model.Questions)
                    {
                        var updateQuestionQuery = @"UPDATE Questions 
                            SET QuestionText = @QuestionText, 
                                OptionA = @OptionA, 
                                OptionB = @OptionB, 
                                OptionC = @OptionC, 
                                OptionD = @OptionD, 
                                CorrectOption = @CorrectOption, 
                                QuestionType = @QuestionType,
                                MinAcceptableValue = @MinAcceptableValue,
                                MaxAcceptableValue = @MaxAcceptableValue
                            WHERE QuestionId = @QuestionId";

                        using (var cmd = new SqlCommand(updateQuestionQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                            cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                            cmd.Parameters.AddWithValue("@OptionA", (object?)question.OptionA ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OptionB", (object?)question.OptionB ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OptionC", (object?)question.OptionC ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OptionD", (object?)question.OptionD ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CorrectOption", (object?)question.CorrectOption ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                            cmd.Parameters.AddWithValue("@MinAcceptableValue", (object?)question.MinAcceptableValue ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaxAcceptableValue", (object?)question.MaxAcceptableValue ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return RedirectToAction("QuizListForTeacher");
            }
            return View("EditQuiz", model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult AddQuiz()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult AddQuiz(QuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Quizzes (Title, Description, TotalQuestions) OUTPUT INSERTED.QuizId VALUES (@Title, @Description, @TotalQuestions)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", model.Title);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        //command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                        command.Parameters.AddWithValue("@TotalQuestions", model.TotalQuestions);
                        var quizId = (int)command.ExecuteScalar();

                        return RedirectToAction("AddQuestion", new { quizId });
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult AddQuestion(int quizId)
        {
            var model = new QuestionViewModel { QuizId = quizId };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult AddQuestion(QuestionViewModel model, string action)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                INSERT INTO Questions (
                    QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
                    CorrectOption, QuestionType, MinAcceptableValue, MaxAcceptableValue
                ) VALUES (
                    @QuizId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, 
                    @CorrectOption, @QuestionType, @MinAcceptableValue, @MaxAcceptableValue
                )";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizId", model.QuizId);
                        command.Parameters.AddWithValue("@QuestionText", model.QuestionText);

                        // MCQ Options
                        command.Parameters.AddWithValue("@OptionA", model.QuestionType == "MCQ" ? (object?)model.OptionA ?? DBNull.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@OptionB", model.QuestionType == "MCQ" ? (object?)model.OptionB ?? DBNull.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@OptionC", model.QuestionType == "MCQ" ? (object?)model.OptionC ?? DBNull.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@OptionD", model.QuestionType == "MCQ" ? (object?)model.OptionD ?? DBNull.Value : DBNull.Value);

                        //command.Parameters.AddWithValue("@CorrectOption", model.CorrectOption);
                        command.Parameters.AddWithValue("@CorrectOption",
                    model.QuestionType == "Numerical" ? (object)DBNull.Value :
                    !string.IsNullOrEmpty(model.CorrectOption) ? model.CorrectOption : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@QuestionType", model.QuestionType);

                        command.Parameters.AddWithValue(
                         "@MinAcceptableValue",
                         model.QuestionType == "Numerical" && model.MinAcceptableValue.HasValue
                             ? model.MinAcceptableValue.Value
                             : (object)DBNull.Value
 );

                        command.Parameters.AddWithValue(
                            "@MaxAcceptableValue",
                            model.QuestionType == "Numerical" && model.MaxAcceptableValue.HasValue
                                ? model.MaxAcceptableValue.Value
                                : (object)DBNull.Value
                        );


                        command.ExecuteNonQuery();
                    }
                }

                // Redirect based on action
                if (action == "Add Another")
                {
                    return RedirectToAction("AddQuestion", new { quizId = model.QuizId });
                }
                else if (action == "Finish")
                {
                    return RedirectToAction("QuizListForTeacher");
                }
            }

            // If model state is invalid, redisplay form with validation messages
            return View("AddQuestion", model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult DeleteQuiz(int quizId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // 1. Delete related quiz results
                var deleteResultsQuery = "DELETE FROM QuizResults WHERE QuizId = @QuizId";
                using (var deleteResultsCommand = new SqlCommand(deleteResultsQuery, connection))
                {
                    deleteResultsCommand.Parameters.AddWithValue("@QuizId", quizId);
                    deleteResultsCommand.ExecuteNonQuery();
                }
                // 2. Delete suspicious activity logs
                var deleteLogsQuery = "DELETE FROM SuspiciousLogs WHERE QuizId = @QuizId";
                using (var deleteLogsCommand = new SqlCommand(deleteLogsQuery, connection))
                {
                    deleteLogsCommand.Parameters.AddWithValue("@QuizId", quizId);
                    deleteLogsCommand.ExecuteNonQuery();
                }

                // 2. Delete associated questions
                var deleteQuestionsQuery = "DELETE FROM Questions WHERE QuizId = @QuizId";
                using (var deleteQuestionsCommand = new SqlCommand(deleteQuestionsQuery, connection))
                {
                    deleteQuestionsCommand.Parameters.AddWithValue("@QuizId", quizId);
                    deleteQuestionsCommand.ExecuteNonQuery();
                }

                // 3. Delete the quiz
                var deleteQuizQuery = "DELETE FROM Quizzes WHERE QuizId = @QuizId";
                using (var deleteQuizCommand = new SqlCommand(deleteQuizQuery, connection))
                {
                    deleteQuizCommand.Parameters.AddWithValue("@QuizId", quizId);
                    deleteQuizCommand.ExecuteNonQuery();
                }
            }

            return RedirectToAction("QuizListForTeacher");
        }

        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));

        }

        [HttpGet]
        [Authorize(Roles = "Student")]
       
        public IActionResult TakeQuiz(int quizId)
        {
            var quiz = _quizService.GetQuizById(quizId);
            if (quiz == null)
            {
                return NotFound();
            }
            var random = new Random();
            quiz.Questions = quiz.Questions
                .OrderBy(q => random.Next())
                .Take(quiz.TotalQuestions)
                .ToList();
            return View(quiz);
        }


        [HttpPost]
        public IActionResult SubmitQuiz(QuizSubmissionViewModel submission)
        {
            // Debugging output to verify received data
            Console.WriteLine($"QuizId: {submission.QuizId}");
            foreach (var answer in submission.UserAnswers)
            {
                Console.WriteLine($"QuestionId: {answer.QuestionId}, Answer: {answer.Answer}");
            }

            if (submission == null || submission.UserAnswers == null || !submission.UserAnswers.Any())
            {
                return BadRequest("Invalid quiz submission.");
            }

            // Prepare user answers dictionary for evaluation
            var userAnswers = submission.UserAnswers
                .ToDictionary(ua => ua.QuestionId, ua => ua.Answer);

            // Evaluate the quiz and calculate the score
            int score = _quizService.EvaluateQuiz(submission.QuizId, userAnswers);

            // Get quiz details for result view
            var quiz = _quizService.GetQuizById(submission.QuizId);
            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }
            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);


            _quizService.SaveQuizResult(submission.QuizId, userId, score);

            //_quizService.SaveQuizResult(submission.QuizId, submission.UserId, score);

            // Prepare the result view model
            var resultViewModel = new QuizResultViewModel
            {
                QuizTitle = quiz.Title,
                TotalQuestions = submission.UserAnswers.Count,

                CorrectAnswers = score,
                Score = (int)((score / (double)submission.UserAnswers.Count) * 100)
            };
 

            // Return the result view with the model
            return View("QuizResult", resultViewModel);
        }

        [HttpGet]
        public IActionResult ViewResults(int quizId)
        {
            var results = _quizService.GetQuizResults(quizId);

            if (results == null || !results.Any())
            {
                return View("QuizResults", new List<QuizResultViewModel>());
            }

            return View("QuizResults", results);
        }



        [HttpPost]
        public IActionResult LogSuspiciousActivity([FromBody] SuspiciousActivityLog log)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertQuery = @"INSERT INTO SuspiciousLogs 
            (QuizId, UserId, EventType, QuestionId, TimeTaken, Timestamp) 
            VALUES (@QuizId, @UserId, @EventType, @QuestionId, @TimeTaken, @Timestamp)";

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuizId", log.QuizId);
                    command.Parameters.AddWithValue("@UserId", log.UserId);
                    command.Parameters.AddWithValue("@EventType", log.EventType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@QuestionId", (object?)log.QuestionId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TimeTaken", (object?)log.TimeTaken ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return Ok();
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult ViewSuspiciousLogs(int quizId)
        {
            List<SuspiciousActivityLog> logs = new List<SuspiciousActivityLog>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM SuspiciousLogs WHERE QuizId = @QuizId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QuizId", quizId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new SuspiciousActivityLog
                            {
                                QuizId = (int)reader["QuizId"],
                                UserId = reader["UserId"].ToString(),
                                EventType = reader["EventType"].ToString(),
                                QuestionId = reader["QuestionId"] as int?,
                                TimeTaken = reader["TimeTaken"] as double?,
                                Timestamp = (DateTime)reader["Timestamp"]
                            });
                        }
                    }
                }
            }

            return View(logs);
        }

    }
}





