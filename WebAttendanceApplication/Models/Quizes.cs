namespace WebAttendanceApplication.Models;

public partial class Quizes
{
    public int QuizId { get; set; }

    public int CourseId { get; set; }

    public string QuizTitle { get; set; } = null!;

    public string QuizPwd { get; set; } = null!;

    public DateTime AvailabeOn { get; set; }

    public DateTime AvailableUntil { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
