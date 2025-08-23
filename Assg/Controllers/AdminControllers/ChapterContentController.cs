using Microsoft.AspNetCore.Mvc;
using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assg.Controllers.AdminControllers
{
    [Route("Admin/ChapterContent")]
    public class ChapterContentController : Controller
    {
        private readonly AppDbContext _context;

        public ChapterContentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ChapterContentManagement")]
        public IActionResult ChapterContentManagement()
        {
            return View();
        }

        [Route("Admin/[controller]")]
        [HttpGet("{courseId}/{chapterId?}")]
        public IActionResult ChapterContent(string courseId, int chapterId = 0)
        {
            var course = _context.Course
                .Include(c => c.Chapters)
                .ThenInclude(ch => ch.ChapterContents)
                .ThenInclude(cc => cc.Reviews)
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("CourseManagement", "Course");
            }

            var chapter = chapterId == 0
                ? course.Chapters.FirstOrDefault()
                : course.Chapters.FirstOrDefault(ch => ch.Id == chapterId);

            if (chapter == null || chapter.ChapterContents == null || !chapter.ChapterContents.Any())
            {
                TempData["ErrorMessage"] = "No chapters available for this course.";
                return RedirectToAction("CourseManagement", "Course");
            }

            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Name;
            ViewBag.ChapterId = chapter.Id;
            ViewBag.ChapterName = chapter.Title;

            var chapterContents = chapter.ChapterContents
                .Select(cc => new
                {
                    cc.Id,
                    cc.Title,
                    Description = cc.Description ?? "No description available",
                    TotalReviewCount = cc.Reviews?.Count() ?? 0
                }).ToList();

            return View("~/Views/AdminViews/Admin/ChapterContentManagement.cshtml", chapterContents);
        }

    }
}