namespace WebAttendanceApplication.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? CourseId { get; set; }

    public int? StudentId { get; set; }

    public int? QuizId { get; set; }

    public string AttendanceStatus { get; set; } = null!;

    public DateTime? Time { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Quizes? Quiz { get; set; }

    public virtual Student? Student { get; set; }
}
