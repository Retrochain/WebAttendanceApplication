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

        /* Written by Akshaan Singh for CS 4485.0W1, Final Project, starting March 20th, 2025
         * Login Controller
         * 
         * This controller tracks the Login information of a student user. There are 3 major
         * parts of this controller that cover the login functionallity. 
         * - Login: The Login View shows the user two fields: UTD ID and Password. When the user
         *          presses the submit button, the Login Constructor checks if the input is valid,
         *          and checks with the database if the input data exists as a record in the database.
         *          There are checks for empty fields, incorrect UTD IDs, incorrect passwords (if they
         *          exist), and a check for no set password (that is, if the student doesnt have a 
         *          password yet in the database). Once the checks are cleared, the user with the 
         *          matching input UTD ID is fetched, and the data for that student is used to create
         *          a cookie for ease of use, and the user is redirected to their courses page.
         *          
         * - Logout: A self explanatory constructor that when redirected to, just logs off the user
         *           and removes their information from the browser cookie. The user is then redirected
         *           to the login screen.
         *           
         * - Set Password: Perhaps the second most important constructor, it is accessed when either A)
         *                 The student has forgotten their password and wants to reset it, or B) When a 
         *                 first time student logs in, they are told to set a password for themselves.
         *                 The View for this constructor displays 3 fields, one for their UTD IDs, one
         *                 for their new password, and then one for them to confirm their password by
         *                 retyping it again. There are once again error checks in place for incorrect 
         *                 UTD IDs, too small passwords, mismatching passwords, and empty fields. When 
         *                 the submit button is pressed in the form, the constructor checks if the UTD
         *                 ID is valid, and upon validity, the password from the new password field is
         *                 saved in that particular student's record. 
         *                 
         * The Login Controller is only concerned with login functionality, as such the logic here is 
         * strictly login based. 
         */


        // The GET constructor for Login page, ensuring that the form is still visible
        [HttpGet]
        public IActionResult Login()
        {
            // We store this text to dynamically update the reset password button
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
                        ViewBag.Password = "Reset Password";
                        ModelState.AddModelError("Password", "Password is required.");
                        return View(model);
                    }
                    else
                    {
                        var stdntPass = _context.Students.Where(p => p.Password == model.Password).FirstOrDefault();
                        if (stdntPass == null)
                        {
                            ViewBag.Password = "Reset Password";
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

                    return RedirectToAction("Courses", "Courses");
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