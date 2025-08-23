using Assg.Entities;
using Assg.Models.TutorModels;
using Microsoft.AspNetCore.Mvc;

namespace Assg.Controllers.TutorControllers;

[Route("Tutor/Quiz")]
public class QuizController : Controller
{
    private readonly AppDbContext _context;

    public QuizController(AppDbContext context)
    {
        _context = context;
    }

    [Route("Answer/{chapterContentId}")]
    public IActionResult Answer(int chapterContentId)
    {
        if (chapterContentId == null)
        {
            TempData["ErrorMessage"] = "Invalid chapter content ID.";
            return RedirectToAction("Index", "Home");
        }
        var quiz = _context.QuizQuestion.Where(q => q.ChapterContentId == chapterContentId).FirstOrDefault();

        if (quiz == null)
        {
            TempData["ErrorMessage"] = "Currently no quiz has been assigned yet.";
            return RedirectToAction("Index", "Home");
        }

        var questions = _context.Questions.Where(q => q.quizId == quiz.Id).ToList();
        quiz.questions = questions;

        return View("~/Views/TutorViews/Quiz/Answer.cshtml", quiz);

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

        var quizAnswer = new Assg.Models.StudentModels.QuizAnswer
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

        return View("~/Views/TutorViews/Quiz/SubmitAnswer.cshtml");

    }


}
