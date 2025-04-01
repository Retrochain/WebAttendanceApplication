using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAttendanceApplication.Data;

namespace WebAttendanceApplication.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CoursesController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Courses()
        {
            ViewBag.Name = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var stdntEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(stdntEmail))
            {
                return RedirectToAction("Login");
            }

            var stdnt = _context.Students.FirstOrDefault(s => s.Email == stdntEmail);

            if (stdnt == null)
            {
                return RedirectToAction("Login");
            }

            var enrolledCourses = _context.Enrollments.Where(e => e.StudentId == stdnt.StudentId).Select(e => new
            {
                e.Course.CourseId,
                e.Course.CourseName,
                e.Course.CourseCode,
                e.Course.ProfName
            })
            .ToList();

            ViewBag.Name = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            return View(enrolledCourses);
        }
    }
}
