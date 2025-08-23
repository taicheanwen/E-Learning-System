using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assg.Controllers.StudentsControllers
{
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;
        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Answer(int? chapterContentId)
        {
            // Check if chapterContentId is null
            if (chapterContentId == null)
            {
                TempData["ErrorMessage"] = "Invalid chapter content ID.";
                return RedirectToAction("Index", "Home"); // Redirect to a general page
            }

            // Find the quiz associated with the chapter content
            var quiz = _context.QuizQuestion
                .FirstOrDefault(q => q.ChapterContentId == chapterContentId);

            // If no quiz is found, display an error message
            if (quiz == null)
            {
                TempData["ErrorMessage"] = "Currently no quiz has been assigned yet.";
                return RedirectToAction("Index", "Home"); // Redirect to a general page
            }

            // Get the questions associated with the quiz
            var questions = _context.Questions
                .Where(q => q.quizId == quiz.Id)
                .ToList();

            quiz.questions = questions;

            // Return the view with the quiz data
            return View("~/Views/StudentViews/Quiz/Answer.cshtml", quiz);
        }



        [HttpGet]
        public IActionResult GetQuizImage(int id)
        {
            var quizQuestion = _context.Questions.Find(id);
            if (quizQuestion == null || quizQuestion.FileData == null || !quizQuestion.FileType.StartsWith("image/"))
            {
                return NotFound();
            }

            return File(quizQuestion.FileData, quizQuestion.FileType);
        }

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            var quizQuestion = _context.Questions.Find(id);
            if (quizQuestion == null || quizQuestion.FileData == null)
            {
                return NotFound();
            }

            return File(quizQuestion.FileData, quizQuestion.FileType, quizQuestion.FileName);
        }

        [HttpPost]
        public IActionResult SubmitAnswers(IFormCollection form)
        {
            var quizId = form["quizId"].FirstOrDefault();
            var userName = form["name"];

            var questions = _context.Questions
                .Where(q => q.quizId == quizId)
                .ToList();

            var userAnswers = new List<(string Question, string UserAnswer, string CorrectAnswer, bool IsCorrect)>();
            int totalScore = 0;

            foreach (var question in questions)
            {
                var formKey = $"answers[{question.Id}].MultipleChoiceAnswer";
                var userAnswer = form[formKey].FirstOrDefault();

                bool isCorrect = userAnswer == question.CorrectAnswer;
                if (isCorrect)
                {
                    totalScore++;
                }

                userAnswers.Add((question.Question, userAnswer ?? "No Answer", question.CorrectAnswer, isCorrect));
            }

            var quizAnswer = new QuizAnswer
            {
                quizId = quizId,
                name = userName,
                score = totalScore,
                createdDate = DateTime.Now
            };

            _context.QuizAnswer.Add(quizAnswer);
            _context.SaveChanges();

            ViewBag.UserScore = totalScore;
            ViewBag.UserAnswers = userAnswers;

            return View("~/Views/StudentViews/Quiz/SubmitAnswers.cshtml");
        }
    }
}
