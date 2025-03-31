using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAttendanceApplication.Data;
using WebAttendanceApplication.Models;

namespace WebAttendanceApplication.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            // Get the logged-in student's email from claims (stored by LoginController)
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(studentEmail))
            {
                return RedirectToAction("Login", "Login"); // Redirect to login if email missing
            }

            // Fetch the student by email 
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Fetch attendance records for this student using their StudentId
            var attendances = await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == student.StudentId)
                .ToListAsync();

            // Calculate summary stats 
            // Counts all records that match the three conditions stored in the database
            ViewBag.TotalAttended = attendances.Count(a => a.AttendanceStatus == "present");
            ViewBag.TotalMissed = attendances.Count(a => a.AttendanceStatus == "absent");
            ViewBag.ExcusedAbsences = attendances.Count(a => a.AttendanceStatus == "excused");
            ViewBag.TotalClasses = attendances.Count;
            //Calculates Attendance Percentage
            ViewBag.Percentage = ViewBag.TotalClasses > 0
                ? (ViewBag.TotalAttended / (double)ViewBag.TotalClasses * 100).ToString("0.00")
                : "0.00";

            // Get course name
            ViewBag.CourseName = attendances.FirstOrDefault()?.Course?.CourseName ?? "[Course Name]";

            return View(attendances);
        }

    }
}
