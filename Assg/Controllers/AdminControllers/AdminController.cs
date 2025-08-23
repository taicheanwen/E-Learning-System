using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assg.Models.StudentModels;
using Assg.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Assg.Controllers.AdminControllers
{
    [Route("Admin")]
    public class AdminAccountController : AdminBaseController
    {
        // GET: Admin Login page
        [HttpGet("AdminLogin")]
        public IActionResult AdminLogin()
        {
            return View("~/Views/AdminViews/Admin/AdminLogin.cshtml");
        }

        // POST: Handle Admin Login
        [HttpPost("AdminLogin")]
        public IActionResult AdminLogin(string Username, string Password)
        {
            const string adminUser = "admin";
            const string adminPassword = "123123";

            if (Username == adminUser && Password == adminPassword)
            {
                HttpContext.Session.SetString("AdminLogged", "true");
                return View("~/Views/AdminViews/Admin/Dashboard.cshtml");
            }
            else
            {
                ViewBag.Error = "Invalid credentials";
                return View("~/Views/AdminViews/Admin/AdminLogin.cshtml");
            }
        }

        // POST: Handle Admin Logout
        [HttpPost("AdminLogout")]
        public IActionResult AdminLogout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AdminLogin", "Admin");
        }

        [HttpPost("Back")]
        public IActionResult Back()
        {
            return View("~/Views/AdminViews/Admin/Dashboard.cshtml");
        }
    }
}

