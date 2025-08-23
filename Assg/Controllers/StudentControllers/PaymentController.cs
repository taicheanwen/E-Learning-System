using Assg.Entities;
using Microsoft.AspNetCore.Mvc;
using Assg.Models.StudentModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Globalization;
namespace Assg.Controllers.StudentsControllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Assg.Helper hp;

        public PaymentController(AppDbContext _context, Assg.Helper hp)
        {
            this._context = _context;
            this.hp = hp;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                TempData["ErrorMessage"] = "Please login to proceed with payment.";
                return RedirectToAction("Login", "Account");
            }

            var userAccount = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                TempData["ErrorMessage"] = "User account not found.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Cart
                                    .Where(u => u.UserAccountId == userAccount.Id)
                                    .Include(c=>c.ChapterContent)
                                    .ToList();

            var subtotal = cartItems.Sum(c => c.Subtotal);
            var cartIds = cartItems.Select(c => c.CartId).ToList();

            ViewData["Subtotal"] = subtotal;
            ViewData["CartId"] = string.Join(",", cartIds);

            return View("~/Views/StudentViews/Account/Payment.cshtml");
        }

        [HttpPost]
        public IActionResult Checkout(Payment payment)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                TempData["ErrorMessage"] = "Please login to proceed with payment.";
                return RedirectToAction("Login", "Account");
            }

            var userAccount = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Cart
                                    .Where(u => u.UserAccountId == userAccount.Id)
                                    .Include(c => c.ChapterContent)
                                    .ToList();

            var cartIds = cartItems.Select(c => c.CartId).ToList();
            var subtotal = cartItems.Sum(c => c.Subtotal);
            ViewData["Subtotal"] = subtotal;
            ViewData["CartId"] = string.Join(",", cartIds);

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty or no items found for this Cart ID.";
                return RedirectToAction("Category", "Category");
            }

            if (payment.CardExpiredDate < DateOnly.FromDateTime(DateTime.Now))
            {
                ModelState.AddModelError("CardExpiredDate", "Card expiration date must not be in the past.");
                return View("~/Views/StudentViews/Account/Payment.cshtml", payment);
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in field {state.Key}: {error.ErrorMessage}");
                    }
                }

                TempData["ErrorMessage"] = "Please fix the errors before submitting the form.";
                return View("~/Views/StudentViews/Account/Payment.cshtml", payment);
            }

            string uppercaseName = payment.CardHolderName.ToUpper();
            foreach (var cartItem in cartItems)
            {
                var newPayment = new Payment
                {
                    UserId = userAccount.Id,
                    PaymentAmount = payment.PaymentAmount,
                    CardNo = payment.CardNo,
                    CardExpiredDate = payment.CardExpiredDate,
                    CVV = payment.CVV,
                    CardHolderName = uppercaseName,
                    ChapterContentId = cartItem.ChapterContentId,
                    PaymentStatus = "Success",
                    PaymentDate = payment.PaymentDate,
                };
                _context.Payment.Add(newPayment);
                cartItem.IsInCart = false;
                _context.SaveChanges();
            }
            var itemsToRemove = cartItems.Where(c => c.IsInCart == false).ToList();
            if (itemsToRemove.Any())
            {
                _context.Cart.RemoveRange(itemsToRemove);
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "Payment processed successfully!";
            return View("~/Views/StudentViews/Account/Payment.cshtml");
        }

        [HttpGet]
        public IActionResult TransactionHistory(string? sort, string? dir)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Please log in to view your transaction history.";
                return RedirectToAction("Login", "Account");
            }

            var userAccount = _context.UserAccount
                .FirstOrDefault(u => u.Username == username);

            if (userAccount == null)
            {
                TempData["ErrorMessage"] = "User account not found.";
                return RedirectToAction("Login", "Account");
            }

            var paymentHistory = _context.Payment
                .Include(p => p.ChapterContent)
                .Where(p => p.UserId == userAccount.Id)
                .ToList();

            ViewBag.Sort = sort;
            ViewBag.Dir = dir;

            Func<Payment, object> fn = sort switch
            {
                "Payment Date" => p => p.PaymentDate,
                "Title" => p => p.ChapterContent.Title,
                "Payment Amount" => p => p.PaymentAmount,
                _ => p => p.PaymentId,
            };

            var sortedPayments = dir == "des" ?
                paymentHistory.OrderByDescending(fn).ToList() :
                paymentHistory.OrderBy(fn).ToList();

            var checkReview = _context.Review
                .Select(r => r.PaymentId)
                .ToList();
            ViewBag.CheckReview = checkReview;


            return View("~/Views/StudentViews/Account/TransactionHistory.cshtml", sortedPayments);
        }

        [HttpGet]
        public IActionResult Review(int paymentId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            string displayUsername = username;
            if (username == null)
            {
                TempData["ErrorMessage"] = "Please login to proceed with rate and review.";
                return RedirectToAction("Login", "Account");
            }

            var userAccount = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                displayUsername = "Unknown";
            }

            var payment = _context.Payment
                .Include(p => p.ChapterContent)
                .FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction("TransactionHistory");
            }
            ViewBag.CourseName = payment.ChapterContent.Title;
            ViewBag.CourseId = payment.ChapterContentId;
            ViewBag.PaymentId = payment.PaymentId;

            int userId = userAccount?.Id ?? 0;
            ViewBag.UserId = userId;

            if (userId == 0)
            {
                displayUsername = "Unknown";
            }

            ViewBag.Username = displayUsername;
            return View("~/Views/StudentViews/Account/Review.cshtml", payment);
        }

        public IActionResult GetReviews()
        {
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.UserAccount
                                      .FirstOrDefault(u => u.Username == currentUserName);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = currentUser.Id;

            var reviews = _context.Review
                .Include(r => r.ChapterContent)
                .Where(r => r.UserId == currentUserId)
                .ToList();

            var reviewData = reviews
                .Select(r => new
                {
                    r.ReviewId,
                    r.ReviewRate,
                    r.ReviewText,
                    r.ReviewDate,
                    CourseName = r.ChapterContent.Title,
                    UserName = currentUser.Username
                }).ToList();

            return View("~/Views/StudentViews/Account/ViewReview.cshtml", reviewData);
        }

        [HttpPost]
        public IActionResult Review(int PaymentId, int ReviewRate, string ReviewText, int CourseId, int UserId)
        {
            var payment = _context.Payment.FirstOrDefault(p => p.PaymentId == PaymentId);
            var course = _context.ChapterContent.FirstOrDefault(c => c.Id == CourseId);
            var user = _context.UserAccount.FirstOrDefault(u => u.Id == UserId);

            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                Console.WriteLine($"Payment : {payment}");
                return RedirectToAction("TransactionHistory");
            }

            if (course == null)
            {
                TempData["ErrorMessage"] = "The course does not exist.";
                Console.WriteLine($"Course : {course}");
                return RedirectToAction("TransactionHistory");
            }

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var review = new Review
            {
                PaymentId = PaymentId,
                ReviewRate = ReviewRate,
                ReviewText = ReviewText,
                ReviewDate = DateTime.Now,
                ChapterContentId = CourseId,
                UserId = UserId,
            };

            Console.WriteLine($"PaymentId : {review.PaymentId}");
            Console.WriteLine($"ReviewRate : {review.ReviewRate}");
            Console.WriteLine($"ReviewText : {review.ReviewText}");
            Console.WriteLine($"ReviewDate : {review.ReviewDate}");
            Console.WriteLine($"Course : {review.ChapterContentId}");
            Console.WriteLine($"UserId : {review.UserId}");

            _context.Review.Add(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Thank you for your review!";
            return RedirectToAction("Review", new { paymentId = PaymentId });
        }

        [HttpPost]
        public IActionResult EditReview(Review model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error in system. Try again later.";
                return RedirectToAction("GetReviews");
            }

            var review = _context.Review.FirstOrDefault(r => r.ReviewId == model.ReviewId);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction("GetReviews");
            }

            review.ReviewRate = model.ReviewRate;
            review.ReviewText = model.ReviewText;
            review.ReviewDate = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("GetReviews");
        }

        [HttpPost]
        public IActionResult DeleteReview(int reviewId)
        {
            var review = _context.Review.FirstOrDefault(r => r.ReviewId == reviewId);

            if (review == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction("GetReviews");
            }
            _context.Review.Remove(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Review deleted successfully.";
            return RedirectToAction("GetReviews");
        }


        [HttpPost]
        public IActionResult Ereceipt(int paymentId,int userId)
        {
            if (ModelState.IsValid)
            {
                var u = _context.UserAccount.FirstOrDefault(u => u.Id == userId);

                if (u == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("TransactionHistory", "Payment");
                }

                var payment = _context.Payment
                    .Where(p => p.PaymentId == paymentId)
                    .Select(p => new
                    {
                        p.PaymentDate,
                        p.ChapterContent.Title,
                        p.ChapterContent.Description,
                        p.PaymentAmount
                    })
                    .FirstOrDefault();

                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Payment not found.";
                    return RedirectToAction("TransactionHistory", "Payment");
                }

                var mail = new MailMessage
                {
                    Subject = "E-receipt",
                    IsBodyHtml = true,
                };

                mail.To.Add(new MailAddress(u.Email, u.Name));

                mail.Body = $@"
                <p>Dear {u.Name},</p>
                <p>This is your e-receipt:</p>
                <table border='1' cellpadding='5' style='border-collapse: collapse;'>
                    <tr>
                        <td><strong>Payment Date:</strong></td>
                        <td>{payment.PaymentDate.ToString("yyyy-MM-dd")}</td>
                    </tr>
                    <tr>
                        <td><strong>Title:</strong></td>
                        <td>{payment.Title}</td>
                    </tr>
                    <tr>
                        <td><strong>Description:</strong></td>
                        <td>{payment.Description}</td>
                    </tr>
                    <tr>
                        <td><strong>Amount:</strong></td>
                        <td>{payment.PaymentAmount.ToString("C", new CultureInfo("ms-MY"))}</td>
                    </tr>
                </table>
                <p>If you have any questions, please don't hesitate to contact us.</p>
                <p>From, <strong>EZLearning Center</strong></p>
                ";

                hp.SendEmail(mail);

                TempData["SuccessMessage"] = "Please check your email for the receipt.";
                return RedirectToAction("TransactionHistory", "Payment");
            }

            return View("~/Views/StudentViews/Account/TransactionHistory.cshtml");
        }


    }
}
