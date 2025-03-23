using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAttendanceApplication.Data;
using WebAttendanceApplication.ViewModels;

namespace WebAttendanceApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public IActionResult Index()
        {
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

            ViewBag.Name = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            return View(enrolledCourses);
        }
        public IActionResult Login()
        {
            return View();
        }

        //Posting to the server once submit is pressed
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            //If the current model is valid
            if (ModelState.IsValid)
            {
                var stdntUser = _context.Students.Where(x => x.UtdId == model.UtdId && x.UserName == model.UserName).FirstOrDefault();
                if (stdntUser != null)
                {

                    bool isEnrolled = _context.Enrollments.Any(e => e.StudentId == stdntUser.StudentId);
                    //if (isEnrolled)
                    //{
                    var claims = new List<Claim>
                        {
                        //Storing the email in a cookie
                        new Claim(ClaimTypes.Email, stdntUser.Email),
                        //Storing the first name in a cookie
                        new Claim(ClaimTypes.Name, stdntUser.FirstName ?? "User"),
                        //Storing the user role
                        new Claim(ClaimTypes.Role, "User"),
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    //}
                    return RedirectToAction("SecurePage");
                }
                else
                {
                    ModelState.AddModelError("", "You are not enrolled in any courses, please contact your professor.");
                }
            }
            else
            {
                ModelState.AddModelError("", "UTD ID or Username is not correct.");
            }
            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            ViewBag.Name = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            return View();
        }
    }
}
