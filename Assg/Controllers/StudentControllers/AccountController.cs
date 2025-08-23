using System.Globalization;
using System.Net.Mail;
using System.Security.Claims;
using Assg.Entities;
using Assg.Models.StudentModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace Assg.Controllers.StudentsControllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment en;
        private readonly Assg.Helper hp;
        private readonly IHttpContextAccessor ct;

        public AccountController(AppDbContext appDbContext, IWebHostEnvironment en, Assg.Helper hp, IHttpContextAccessor ct)
        {
            _context = appDbContext;
            this.en = en;
            this.hp = hp;
            this.ct = ct;
        }

        public IActionResult Index()
        {
            return View(_context.UserAccount.ToList());
        }

        public IActionResult Registration()
        {
            return View("~/Views/StudentViews/Account/Registration.cshtml");
        }


        [HttpPost]
        public IActionResult Registration(StudentRegister model)
        {
            if (ModelState.IsValid)
            {
                if (_context.UserAccount.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "The username has been created, please choose another.");
                }
                if (_context.UserAccount.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email has already been used, please try another.");
                }

                if (!ModelState.IsValid)
                {
                    return View("~/Views/StudentViews/Account/Registration.cshtml", model);
                }

                UserAccount account = new UserAccount
                {
                    Email = model.Email,
                    Username = model.Username,
                    Name = model.Name,
                    Password = new PasswordHasher<UserAccount>().HashPassword(null, model.Password),
                    Profile = "/images/default-profile-image.png",
                    Role = model.Role,
                };

                try
                {
                    _context.UserAccount.Add(account);
                    _context.SaveChanges();
                    ModelState.Clear();
                    ViewBag.Message = $"{account.Name} registered successfully. Please login on the login page.";
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                    Console.WriteLine($"Error message is : {ex}");
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                    return View(model);
                }

                return View("~/Views/StudentViews/Account/Registration.cshtml");
            }

            return View("~/Views/StudentViews/Account/Registration.cshtml", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/StudentViews/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(StudentLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.UserAccount
                    .FirstOrDefault(x => x.Username == model.UsernameOrEmail || x.Email == model.UsernameOrEmail);

                if (user != null)
                {

                    var passwordHasher = new PasswordHasher<UserAccount>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    string profileImage = user.Profile;
                    ViewData["ProfileImage"] = profileImage;
                    Console.WriteLine($"Photo : {profileImage}");

                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("Username", user.Username),
                    new Claim("Name", user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("ProfileImage", user.Profile ?? "default-profile-image.png")
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        if (user.Role.Equals("Tutor", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("MyCourse", "MyCourse");
                        }
                        else if (user.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Category", "Category");
                        }
                        else
                        {
                            return RedirectToAction("Login", "Account");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", " Password is incorrect.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Username/Email is incorrect.");
                }
            }

            return View("~/Views/StudentViews/Account/Login.cshtml", model);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            ViewBag.Name = HttpContext.User.Identity.Name;
            return View();
        }

        public IActionResult VerifyEmail()
        {
            return View("~/Views/StudentViews/Account/VerifyEmail.cshtml");
        }

        //[HttpPost]
        //public async Task<IActionResult> VerifyEmail(StudentVerifyEmail model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _context.UserAccount.FirstOrDefaultAsync(u => u.Email == model.Email);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError("", "This email does not exist. Please try again.");
        //            return View(model);
        //        }
        //        else
        //        {
        //            return RedirectToAction("ChangePassword", "Account", new { username = user.Username });
        //        }
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public IActionResult CheckEmail(StudentVerifyEmail model)
        {
            if (ModelState.IsValid)
            {
                var u = _context.UserAccount.FirstOrDefault(u => u.Email == model.Email);

                if (u == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("TransactionHistory", "Payment");
                }

                var mail = new MailMessage
                {
                    Subject = "Change Password",
                    IsBodyHtml = true,
                };

                mail.To.Add(new MailAddress(u.Email, u.Name));

                var resetPasswordUrl = Url.Action("ChangePassword", "Account", new { email = u.Email }, protocol: Request.Scheme);

                mail.Body = $@"
                    <p>Dear {u.Name},</p>
                    <p>Click <a href='{resetPasswordUrl}'>here</a> to change your password.</p>
                    <p>From, <strong>EZLearning Center</strong></p>
                    ";

                hp.SendEmail(mail);

                TempData["SuccessMessage"] = "Please check your email for change password.";
                return View("~/Views/StudentViews/Account/VerifyEmail.cshtml");
            }

            return View("~/Views/StudentViews/Account/VerifyEmail.cshtml");
        }

        [HttpGet]
        public IActionResult ChangePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid email address.";
                return RedirectToAction("VerifyEmail", "Account");
            }

            var user = _context.UserAccount.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View("~/Views/StudentViews/Account/ChangePassword.cshtml",new StudentChangePassword { Email = user.Email });
        }



        [HttpPost]
        public IActionResult ChangePassword(StudentChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.UserAccount.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<UserAccount>();
                    user.Password = passwordHasher.HashPassword(user, model.NewPassword);
                    _context.UserAccount.Update(user);
                    _context.SaveChanges();
                    //user.Password = model.NewPassword; // Store the plain text password directly (not hashed)
                    //_context.UserAccount.Update(user);
                    //_context.SaveChanges();

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }

            return View("~/Views/StudentViews/Account/ChangePassword.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.UserAccount.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new UserAccount
            {
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Profile = user.Profile
            };

            return View("~/Views/StudentViews/Account/EditProfile.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditProfile(UserAccount model, IFormFile photo)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.UserAccount.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }

            string photoFileName = user.Profile;

            if (photo != null)
            {
                var validationResult = hp.ValidatePhoto(photo);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return Json(new { success = false, message = validationResult });
                }

                photoFileName = hp.SavePhoto(photo, "userProfile");
                user.Profile = photoFileName;
            }

            var existingEmailUser = _context.UserAccount
                .FirstOrDefault(u => u.Email == model.Email && u.Username != username);

            if (existingEmailUser != null)
            {
                TempData["ErrorMessage"] = "This email is already registered. Please try another.";
                return RedirectToAction("EditProfile", "Account");
            }

            user.Name = model.Name;
            user.Email = model.Email;

            _context.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Username", user.Username),
                new Claim("Name", user.Name),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("ProfileImage", user.Profile ?? "default-profile-image.png")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Category", "Category");
        }


        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.UserAccount.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var reviews = _context.Review.Where(r => r.UserId == user.Id).ToList();
            foreach (var review in reviews)
            {
                review.UserId = 0;
            }

            _context.UserAccount.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Logout", "Account");
        }

        [HttpGet]
        public IActionResult PurchasedVideos()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.UserAccount.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return Unauthorized();
            }

            var purchasedCourses = _context.Payment
                .Where(p => p.UserId == user.Id)
                .Include(p => p.ChapterContent)
                .ToList();


            return View("~/Views/StudentViews/Account/PurchasedVideo.cshtml", purchasedCourses);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View("~/Views/StudentViews/Account/ContactUs.cshtml");
        }

    }
}
