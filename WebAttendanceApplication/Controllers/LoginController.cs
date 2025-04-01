using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Password = "Reset Password";
            return View();
        }

        //Posting to the server once submit is pressed
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            //If the current model is valid
            if (ModelState.IsValid)
            {
                var stdntUser = _context.Students.Where(x => x.UtdId == model.UtdId).FirstOrDefault();
                if (stdntUser != null)
                {
                    if (model.Password == null)
                    {
                        if (stdntUser.Password == null)
                        {
                            ViewBag.Password = "Create Password";
                            ModelState.AddModelError("Password", "Password not yet set, please create a new password");
                        }
                        ModelState.AddModelError("Password", "Password is required.");
                        return View(model);
                    }
                    else
                    {
                        var stdntPass = _context.Students.Where(p => p.Password == model.Password).FirstOrDefault();
                        if (stdntPass == null)
                        {
                            ModelState.AddModelError("Password", "Password is incorrect.");
                            return View(model);
                        }
                    }
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

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("UtdId", "Student not found, please contact your professor.");
                }
            }
            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetPassword(SetPasswordViewModel spmodel)
        {
            if (ModelState.IsValid)
            {
                var spCheck = _context.Students.Where(s => s.UtdId == spmodel.utdID).FirstOrDefault();
                if (spCheck != null)
                {
                    spCheck.Password = spmodel.NewPassword;
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("utdID", "Incorrect UTD ID entered.");
                    return View(spmodel);
                }
            }
            return View();
        }
    }
}