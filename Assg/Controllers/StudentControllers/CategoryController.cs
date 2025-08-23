using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Assg.Controllers.StudentsControllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Category()
        {
            var categories = _context.Category.ToList();

            var popularVideos = _context.Payment
                .Include(p => p.Course)
                .Where(p => p.PaymentStatus == "Success")
                .GroupBy(p => p.ChapterContentId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    CourseName = g.Select(p => p.ChapterContent.Title).FirstOrDefault(),
                    CourseVideo = g.Select(p => p.ChapterContent.FileType).FirstOrDefault(),
                    PurchaseCount = g.Count()
                })
                .OrderByDescending(v => v.PurchaseCount)
                .Take(3)
                .ToList();

            ViewBag.PopularVideos = popularVideos;
            return View("~/Views/StudentViews/Account/Homepage.cshtml", categories);
        }
        public IActionResult GetSubcategories(string categoryId)
        {
            var categories = _context.Category.ToList();

            var popularVideos = _context.Payment
                .Include(p => p.Course)
                .Where(p => p.PaymentStatus == "Success")
                .GroupBy(p => p.ChapterContentId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    CourseName = g.Select(p => p.ChapterContent.Title).FirstOrDefault(),
                    CourseVideo = g.Select(p => p.ChapterContent.FileType).FirstOrDefault(),
                    PurchaseCount = g.Count()
                })
                .OrderByDescending(v => v.PurchaseCount)
                .Take(3)
                .ToList();

            ViewBag.PopularVideos = popularVideos;

            if (string.IsNullOrEmpty(categoryId))
            {
                TempData["ErrorMessage"] = "Category ID is required";
                return RedirectToAction("Category");
            }

            var subcategories = _context.Category
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new { c.SubcategoryId, c.SubcategoryName })
                .Distinct()
                .ToList();

            var coursecategory = _context.Course
                .Where(c => c.categoryId == categoryId)
                .Select(c => new { c.Id, c.Name })
                .Distinct()
                .ToList();

            ViewBag.Subcategories = coursecategory;

            return View("~/Views/StudentViews/Account/Homepage.cshtml", categories);
        }
    }
}
