using Microsoft.AspNetCore.Mvc;
using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assg.Controllers.AdminControllers
{
    public class CourseController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        [Route("Admin/[controller]")]
        [HttpGet]
        public IActionResult CourseManagement()
        {
            var courses = _context.Course
                .Include(c => c.Category)
                .ToList();

            return View("~/Views/AdminViews/Admin/CourseManagement.cshtml", courses);
        }

        // edit course
        [Route("Admin/[controller]")]
        [HttpPost]
        public IActionResult EditCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                var course = _context.Course.Include(c => c.Category).FirstOrDefault(c => c.Id == model.Id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction("CourseManagement");
                }

                var category = _context.Category.Include(c => c.Courses)
                    .FirstOrDefault(c => c.CategoryId == model.categoryId);

                if (category == null)
                {
                    TempData["EditErrorMessage"] = "Invalid category ID.";
                    TempData["EditCourseId"] = model.Id;
                    return RedirectToAction("CourseManagement");
                }

                if (category.Courses.Any(c => c.Id != model.Id))
                {
                    TempData["EditErrorMessage"] = "Category already assigned to another course.";
                    TempData["EditCourseId"] = model.Id;
                    return RedirectToAction("CourseManagement");
                }

                course.Name = model.Name;
                course.Price = model.Price;
                course.categoryId = model.categoryId;
                course.subtitle = model.subtitle;
                course.description = model.description;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Course updated successfully.";
                return RedirectToAction("CourseManagement");
            }

            TempData["ErrorMessage"] = "Invalid input.";
            return RedirectToAction("CourseManagement");
        }

        // delete course
        [Route("Admin/[controller]")]
        [HttpPost]
        public IActionResult DeleteCourse(string id)
        {
            var course = _context.Course.Include(c => c.Chapters).ThenInclude(ch => ch.ChapterContents).FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("CourseManagement");
            }

            var hasChapterContents = course.Chapters != null && course.Chapters.Any(ch => ch.ChapterContents != null && ch.ChapterContents.Any());
            if (hasChapterContents)
            {
                TempData["ErrorMessage"] = "Course cannot be deleted because it has associated chapter content.";
                return RedirectToAction("CourseManagement");
            }

            _context.Course.Remove(course);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Course deleted successfully.";
            return RedirectToAction("CourseManagement");
        }
    }
}
