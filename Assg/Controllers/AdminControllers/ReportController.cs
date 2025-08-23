using Microsoft.AspNetCore.Mvc;
using Assg.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Assg.Models.StudentModels;

namespace Assg.Controllers.AdminControllers
{
    public class ReportController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        [Route("Admin/[controller]")]
        [HttpGet]
        public IActionResult Report(string? sort, string? dir, string? table)
        {
            ViewBag.Sort = sort;
            ViewBag.Dir = dir;


            // Total Enrolled Student report
            if (table == "student" || string.IsNullOrEmpty(table))
            {
                var students = _context.UserAccount
                    .Where(x => x.Role == "Student")
                    .ToList();

                Func<UserAccount, object> studentSortFn = sort switch
                {
                    "Id" => s => s.Id,
                    "Name" => s => s.Name,
                    "Email" => s => s.Email,
                    "Username" => s => s.Username,
                    _ => s => s.Id,
                };

                var sortedStudents = dir == "des"
                    ? students.OrderByDescending(studentSortFn).ToList()
                    : students.OrderBy(studentSortFn).ToList();

                ViewBag.Students = sortedStudents;
            }


            // Total Enrolled Tutor report
            if (table == "tutors" || string.IsNullOrEmpty(table))
            {
                var tutors = _context.UserAccount
                    .Where(x => x.Role == "Tutor")
                    .ToList();

                Func<UserAccount, object> tutorSortFn = sort switch
                {
                    "Id" => t => t.Id,
                    "Name" => t => t.Name,
                    "Email" => t => t.Email,
                    "Username" => t => t.Username,
                    _ => t => t.Id,
                };

                var sortedTutors = dir == "des"
                    ? tutors.OrderByDescending(tutorSortFn).ToList()
                    : tutors.OrderBy(tutorSortFn).ToList();

                ViewBag.Tutors = sortedTutors;
            }


            // The Most Popular Participant
            if (table == "courses" || string.IsNullOrEmpty(table))
            {
                var courses = _context.Course.ToList();

                Func<Course, object> courseSortFn = sort switch
                {
                    "Id" => c => c.Id,
                    "Name" => c => c.Name,
                    "Price" => c => c.Price,
                    "Participant" => c => c.Participant,
                    _ => c => c.Id,
                };

                var sortedCourses = dir == "des"
                    ? courses.OrderByDescending(courseSortFn).ToList()
                    : courses.OrderBy(courseSortFn).ToList();

                ViewBag.PopularCourses = sortedCourses;
            }

            return View("~/Views/AdminViews/Admin/ReportManagement.cshtml");
        }
    }
}