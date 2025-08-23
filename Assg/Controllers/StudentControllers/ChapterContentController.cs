using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assg.Controllers.StudentsControllers
{
    public class ChapterContentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Assg.Helper hp;

        public ChapterContentController(AppDbContext context, Assg.Helper hp)
        {
            _context = context;
            this.hp = hp;
        }

        [HttpGet]
        public IActionResult ChapterContent(string subcategoryId)
        {
            Console.WriteLine($"Received subcategoryId: {subcategoryId}");

            var query = _context.ChapterContent
                .Include(c => c.Carts)
                .Include(r => r.Reviews)
                .Include(c => c.Chapter)
                    .ThenInclude(chapter => chapter.Course)
                        .ThenInclude(course => course.Category)
                .Where(c => c.Chapter.Course.Id == subcategoryId);

            var contents = query.ToList();
            Console.WriteLine($"Found {contents.Count} chapter contents");

            var contentViewModel = contents.Select(content => new ChapterContentVM
            {
                CourseId = content.Id,
                CourseName = content.Title,
                CourseDescription = content.Description,
                CoursePrice = content.Chapter.Course.Price,
                VideoPath = content.FileType,
                UploadDateTime = content.Chapter.Course.createdDate,
                SubcategoryId = content.Chapter.Course.Id,
                Carts = content.Carts,
                Reviews = content.Reviews,
                ChatQuestion = content.ChatQuestion
            }).ToList();


            Console.WriteLine($"Created {contentViewModel.Count} view models");

            if (contentViewModel.Any())
            {
                var first = contentViewModel.First();
                Console.WriteLine($"First content details: Name={first.CourseName}, Path={first.VideoPath}");
            }
            else
            {
                Console.WriteLine("No content found for this subcategory");
                TempData["ErrorMessage"] = "No content found for this subcategory.";
            }

            var userNames = _context.UserAccount
                .ToDictionary(u => u.Id, u => u.Username);
            ViewBag.UserNames = userNames;

            ViewBag.SubcategoryId = subcategoryId;

            return View("~/Views/StudentViews/Account/ChapterContent.cshtml", contentViewModel);
        }

        [HttpGet]
        public IActionResult ViewCourse(int courseId, string subcategoryId)
        {
            var courses = _context.ChapterContent
                .Include(c => c.Carts)
                .Include(r => r.Reviews)
                .Include(cr => cr.Chapter)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(ct => ct.Category)
                .FirstOrDefault(c => c.Id == courseId);

            var coursecategory = _context.Course
                .Where(c => c.categoryId == subcategoryId)
                .Select(c => new { c.Id, c.Name })
                .Distinct()
                .ToList();

            var userNames = _context.UserAccount
                .ToDictionary(u => u.Id, u => u.Username);

            var viewModel = new ChapterContentVM
            {
                CourseId = courses.Id,
                CourseName = courses.Title,
                CourseDescription = courses.Description,
                Carts = courses.Carts,
                Reviews = courses.Reviews,
                VideoPath = courses.FileType,
                UploadDateTime = courses.Chapter.Course.createdDate,
                CoursePrice = courses.Chapter.Course.Price,
                SubcategoryId = courses.Chapter.Course.Id,
                SubcategoryName = courses.Chapter.Course.Name,
            };

            Console.WriteLine($"SubcategoryId for CourseDetail.cshtml:{viewModel.SubcategoryId}");
            Console.WriteLine($"Course Id for CourseDetail.cshtml:{viewModel.CourseId}");

            ViewBag.UserNames = userNames;
            ViewBag.Subcategories = coursecategory;

            return View("~/Views/StudentViews/Account/CourseDetail.cshtml", viewModel);
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var userId = _context.UserAccount
                                 .FirstOrDefault(u => u.Username == User.Identity.Name)?.Id;

            if (userId == null)
            {
                return Unauthorized("User not found.");
            }

            var cartItems = _context.Cart
                                    .Where(c => c.UserAccountId == userId)
                                    .Include(ch => ch.ChapterContent)
                                    .ToList();

            var cartCourseIds = cartItems.Select(c => c.ChapterContentId).ToList();

            foreach (var cartItem in cartItems)
            {
                cartItem.Subtotal = cartItem.ChapterContentPrice;
                //cartItem.IsInCart = cartCourseIds.Contains(cartItem.CourseId);
            }

            return View("~/Views/StudentViews/Account/Cart.cshtml", cartItems);
        }

        [HttpPost]
        public IActionResult UpdateCart(int chapterContentId, string name, string description, double price, string? subcategoryId)
        {
            Console.WriteLine($"ChapterContentId: {chapterContentId}");
            Console.WriteLine($"SubcategoryId: {subcategoryId}");
            Console.WriteLine($"Price of content: {price}");
            Console.WriteLine($"Name: {name}");
            Console.WriteLine($"Description: {description}");
            var username = User.Identity.Name;

            var userAccount = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                TempData["ErrorMessage"] = "Please login/register first.";
                return RedirectToAction("ChapterContent", new { subcategoryId });

            }

            var userId = userAccount.Id;
            var existingPurchasedItem = _context.Payment.Any(p => p.UserId == userId && p.ChapterContentId == chapterContentId);
            if (existingPurchasedItem)
            {
                TempData["ErrorMessage"] = "You have already purchased this course.Please check your purchased videos/transaction history";
                return RedirectToAction("ChapterContent", new { subcategoryId });
            }


            var course = _context.ChapterContent.FirstOrDefault(c => c.Id == chapterContentId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Content not found.";
                return RedirectToAction("ChapterContent", new { subcategoryId });
            }
            var existingCartItem = _context.Cart.FirstOrDefault(c => c.UserAccountId == userAccount.Id && c.ChapterContentId == chapterContentId);

            if (existingCartItem != null)
            {
                TempData["ErrorMessage"] = "This course is already in your cart.";
                return RedirectToAction("ChapterContent", new { subcategoryId });
            }
            else
            {
                var newCartItem = new Cart
                {
                    UserAccountId = userAccount.Id,
                    ChapterContentId = chapterContentId,
                    ChapterContentPrice = price,
                    ChapterContentDescription = description,
                    ChapterContentName = name,
                    Subtotal = price,
                    IsInCart = true
                };

                _context.Cart.Add(newCartItem);
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Course added successfully!";
            return RedirectToAction("ChapterContent", new { subcategoryId });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int chapterContentId)
        {
            var username = User.Identity.Name;

            if (username == null)
            {
                return Unauthorized("User not found.");
            }

            var userAccount = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                return Unauthorized("User account not found.");
            }

            var cartItem = _context.Cart.FirstOrDefault(c => c.UserAccountId == userAccount.Id && c.ChapterContentId == chapterContentId);

            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Course removed from cart successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Course not found in cart.";
            }

            return RedirectToAction("Cart", new { chapterContentId });
        }

        [HttpGet]
        public IActionResult QnA()
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

            var getCourseId = _context.Payment
                .Include(cp => cp.ChapterContent)
                .Where(p => p.UserId == currentUserId && p.PaymentStatus == "Success")
                .Select(cp => new { cp.ChapterContentId, cp.ChapterContent.Title })
                .ToList();

            ViewData["GetCourseId"] = new SelectList(getCourseId, "ChapterContentId", "Title");
            var selectList = ViewData["GetCourseId"] as SelectList;

            if (selectList != null)
            {
                foreach (var item in selectList)
                {
                    Console.WriteLine($"ChapterContentId: {item.Value}, Title: {item.Text}");
                }
            }
            else
            {
                Console.WriteLine("ViewData[\"GetCourseId\"] is null.");
            }

            var questionIds = _context.ChatQuestion
                .Select(p => p.Id)
                .ToList();

            var replies = _context.ChatQuestion
                .Where(q => questionIds.Contains(q.Id))
                .SelectMany(q => q.Answers.DefaultIfEmpty())
                .Select(a => a.Content ?? "No reply")
                .ToList();

            var questions = _context.ChatQuestion
                        .Where(q => q.UserId == currentUserId)
                        .Select(q => new
                        {
                            q.Id,
                            q.ChapterContentId,
                            q.QuestionTitle,
                            q.QuestionContent,
                            q.CreatedAt,
                            replies = q.Answers.Any() ? string.Join(", ", q.Answers.Select(a => a.Content ?? "No reply")) : "No reply"
                        })
                        .ToList();

            foreach (var question in questions)
            {
                Console.WriteLine($"Question Id : {question.Id}");
                Console.WriteLine($"Chapter Content Id : {question.ChapterContentId}");
                Console.WriteLine($"Title : {question.QuestionTitle}");
                Console.WriteLine($"Content : {question.QuestionContent}");
                Console.WriteLine($"Created at : {question.CreatedAt}");
                Console.WriteLine($"Reply : {question.replies}");
            }

            ViewData["Questions"] = questions;

            if (questions == null || !questions.Any())
            {
                Console.WriteLine("No questions found in the database.");
            }
            else
            {
                Console.WriteLine("Contain questions inside.");
            }


            return View("~/Views/StudentViews/Account/QnA.cshtml");
        }


        [HttpPost]
        public IActionResult QnA(string title, string content, int chapterContentId)
        {
            Console.WriteLine($"Chapter content Id :{chapterContentId} ");
            Console.WriteLine($"Question title :{title} ");
            Console.WriteLine($"Question content :{content} ");

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

            if (chapterContentId == null || chapterContentId == 0)
            {
                TempData["ErrorMessage"] = "Content not found.";
                return RedirectToAction("QnA", new { chapterContentId });
            }

            var course = _context.ChapterContent.FirstOrDefault(c => c.Id == chapterContentId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "The selected content does not exist.";
                return RedirectToAction("QnA", new { chapterContentId });
            }

            var question = new ChatQuestion
            {
                QuestionTitle = title,
                QuestionContent = content,
                UserId = currentUserId,
                CreatedAt = DateTime.Now,
                ChapterContentId = chapterContentId,
            };

            _context.ChatQuestion.Add(question);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Question submitted successfully!";
            return RedirectToAction("QnA");
        }

        [HttpPost]
        public IActionResult DeleteQnA(int questionId)
        {
            var questionToDelete = _context.ChatQuestion.FirstOrDefault(q => q.Id == questionId);

            if (questionToDelete != null)
            {
                _context.ChatQuestion.Remove(questionToDelete);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Question deleted successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "Question not found.";
            }

            return RedirectToAction("QnA");
        }

    }
}
