using Microsoft.AspNetCore.Mvc;
using Assg.Entities;
using Assg.Models.StudentModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Assg.Controllers.AdminControllers
{
    public class CategoryController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }


        [Route("Admin/[controller]")]
        [HttpGet]
        public IActionResult CategoryManagement()
        {
            var categories = _context.Category.ToList();
            return View("~/Views/AdminViews/Admin/CategoryManagement.cshtml", categories);
        }


        //add category
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                TempData["ErrorMessage"] = "Category Name is required.";
                return RedirectToAction("CategoryManagement");
            }

            var lastCategory = _context.Category
                .OrderByDescending(c => c.CategoryId)
                .FirstOrDefault();

            string newId = "C001";
            if (lastCategory != null)
            {
                // Extract numeric part and increment by 1
                int numericPart = int.Parse(lastCategory.CategoryId.Substring(1));
                newId = $"C{(numericPart + 1).ToString("D3")}";
            }

            category.CategoryId = newId;

            _context.Category.Add(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category added successfully.";
            return RedirectToAction("CategoryManagement");
        }

        //edit category
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryId))
            {
                TempData["ErrorMessage"] = "Invalid Category ID.";
                return RedirectToAction("CategoryManagement");
            }

            var existingCategory = _context.Category.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("CategoryManagement");
            }

            existingCategory.CategoryName = category.CategoryName;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction("CategoryManagement");
        }


        // delete category
        [HttpPost]
        public IActionResult DeleteCategory(string categoryId)
        {
            var category = _context.Category
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.SubcategoryId))
            {
                TempData["ErrorMessage"] = "Category cannot be deleted because it has associated subcategories.";
                return RedirectToAction("CategoryManagement");
            }

            _context.Category.Remove(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction("CategoryManagement");
        }

        // subcategory
        [HttpGet("Admin/Subcategory")]
        public IActionResult Subcategory(string categoryId)
        {
            var category = _context.Category.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = category.CategoryId;
            ViewBag.CategoryName = category.CategoryName;

            // Check if any subcategory exists
            var subcategories = _context.Category
                .Where(c => c.CategoryId.StartsWith(categoryId) && c.SubcategoryId != null)
                .Select(c => new { c.SubcategoryId, c.SubcategoryName })
                .ToList();

            bool hasSubcategories = subcategories.Any();

            // Pass whether subcategories exist to the view
            ViewBag.HasSubcategories = hasSubcategories;

            return View("~/Views/AdminViews/Admin/Subcategory.cshtml", subcategories);
        }

        // add subcategory
        [HttpPost]
        public IActionResult AddSubcategory(string categoryId, string subcategoryName)
        {
            if (string.IsNullOrWhiteSpace(subcategoryName))
            {
                TempData["ErrorMessage"] = "Subcategory name cannot be empty.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            var category = _context.Category.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            if (!string.IsNullOrEmpty(category.SubcategoryId))
            {
                TempData["ErrorMessage"] = "This category already has a subcategory. Only one subcategory is allowed per category.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            var maxSubcategoryId = _context.Category
                .Where(c => c.SubcategoryId != null)
                .Select(c => c.SubcategoryId)
                .OrderByDescending(id => id)
                .FirstOrDefault();

            string newSubcategoryId = "SS001";
            if (maxSubcategoryId != null)
            {
                int maxSubcategoryNumber = int.Parse(maxSubcategoryId.Substring(2));
                newSubcategoryId = $"SS{(maxSubcategoryNumber + 1).ToString("D3")}";
            }

            category.SubcategoryId = newSubcategoryId;
            category.SubcategoryName = subcategoryName;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Subcategory added successfully.";
            return RedirectToAction("Subcategory", new { categoryId });
        }

        // edit subcategory
        [HttpPost]
        public IActionResult EditSubcategory(string categoryId, string subcategoryId, string subcategoryName)
        {
            if (string.IsNullOrWhiteSpace(subcategoryId) || string.IsNullOrWhiteSpace(categoryId))
            {
                TempData["ErrorMessage"] = "Invalid Subcategory or Category ID.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            var subcategory = _context.Category.FirstOrDefault(c => c.CategoryId == categoryId && c.SubcategoryId == subcategoryId);
            if (subcategory == null)
            {
                TempData["ErrorMessage"] = "Subcategory not found.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            subcategory.SubcategoryName = subcategoryName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Subcategory updated successfully.";
            return RedirectToAction("Subcategory", new { categoryId });
        }

        // delete subcategory
        [HttpPost]
        public IActionResult DeleteSubcategory(string categoryId, string subcategoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId) || string.IsNullOrWhiteSpace(subcategoryId))
            {
                TempData["ErrorMessage"] = "Invalid Category or Subcategory ID.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            var subcategory = _context.Category.FirstOrDefault(c => c.CategoryId == categoryId && c.SubcategoryId == subcategoryId);
            if (subcategory == null)
            {
                TempData["ErrorMessage"] = "Subcategory not found.";
                return RedirectToAction("Subcategory", new { categoryId });
            }

            subcategory.SubcategoryId = null;
            subcategory.SubcategoryName = null;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Subcategory deleted successfully.";
            return RedirectToAction("Subcategory", new { categoryId });
        }
    }
}
