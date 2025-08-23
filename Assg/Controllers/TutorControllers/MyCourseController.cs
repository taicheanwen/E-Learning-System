using Microsoft.AspNetCore.Mvc;
using Assg.Models.TutorModels;
using Microsoft.EntityFrameworkCore;
using Assg.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Assg.Controllers.TutorControllers;

[Authorize(Roles = "Tutor")]
public class MyCourseController : Controller
{
    private readonly AppDbContext _context;

    public MyCourseController(AppDbContext context)
    {
        _context = context;

    }

    public IActionResult MyCourse()
    {

        var courses = _context.Course.Select(course => new Models.StudentModels.Course
        {
            Id = course.Id,
            Name = course.Name,
            Participant = course.Participant,
        }).ToList();

        var categories = _context.Category.Select(category => new
        {
            categoryId = category.CategoryId,
            categoryName = category.CategoryName

        }).ToList();

        var viewModel = new CourseDetailsViewModel
        {
            Courses = courses,
            Course = new Models.StudentModels.Course()
        };

        ViewBag.Categories = categories;

        return View("~/Views/TutorViews/MyCourse.cshtml", viewModel);
    }

    [HttpPost]
    public IActionResult AddCourse(string name, string categoryId, string subtitle, decimal price, string description)
    {
        if (ModelState.IsValid)
        {
            var maxCourse = _context.Course
                            .OrderByDescending(c => c.Id)
                            .FirstOrDefault();

            string newId;

            if (maxCourse == null)
            {
                newId = "S1001";
            }
            else
            {
                string numericPart = maxCourse.Id.Substring(1);
                int numericId = int.Parse(numericPart);
                numericId++;

                newId = "S" + numericId.ToString();
            }

            var newCourse = new Models.StudentModels.Course
            {
                Id = newId,
                Name = name,
                categoryId = categoryId,
                subtitle = subtitle,
                Price = price,
                description = description,
                Participant = 0,
                createdDate = DateTime.Now
            };

            _context.Course.Add(newCourse);
            _context.SaveChanges();

            return RedirectToAction("MyCourse");
        }

        var categories = _context.Category
        .Select(c => new
        {
            c.CategoryId,
            c.CategoryName
        })
        .Distinct()
        .ToList();

            ViewBag.Categories = categories;

            return RedirectToAction("MyCourse");
    }

    public IActionResult GetSubcategories(string categoryId)
    {
        var subcategories = _context.Category
            .Where(c => c.CategoryId == categoryId)
            .Select(c => new
            {
                c.SubcategoryName
            })
            .Distinct()
            .ToList();

        return Json(subcategories);
    }

    public IActionResult SearchCourses(string searchTerm)
    {
        var filteredCourses = _context.Course
            .Where(c => c.Name.Contains(searchTerm) || c.subtitle.Contains(searchTerm))
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.subtitle
            })
            .ToList();

        return Json(filteredCourses);
    }
}
