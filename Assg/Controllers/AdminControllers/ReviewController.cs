using Microsoft.AspNetCore.Mvc;
using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assg.Controllers.AdminControllers
{
    [Route("Admin/Review")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ReviewManagement/{chapterContentId}")]
        public IActionResult ReviewManagement(int chapterContentId)
        {
            var reviews = _context.Review
                .Where(r => r.ChapterContentId == chapterContentId)
                .Select(r => new
                {
                    r.ReviewId,
                    r.ReviewRate,
                    r.ReviewText,
                    r.ReviewDate,
                    r.PaymentId,
                    r.UserId,
                }).ToList();

            if (!reviews.Any())
            {
                TempData["ErrorMessage"] = "No reviews available for this chapter content.";
            }

            ViewBag.ChapterContentId = chapterContentId;
            return View("~/Views/AdminViews/Admin/ReviewManagement.cshtml", reviews);
        }

        // delete review
        [HttpPost("Delete/{reviewId}")]
        public IActionResult Delete(int reviewId)
        {
            var review = _context.Review.Find(reviewId);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction("ReviewManagement", new { chapterContentId = review?.ChapterContentId });
            }

            _context.Review.Remove(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Review deleted successfully.";
            return RedirectToAction("ReviewManagement", new { chapterContentId = review.ChapterContentId });
        }
    }
}
