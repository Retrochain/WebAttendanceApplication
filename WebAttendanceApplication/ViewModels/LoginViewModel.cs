namespace WebAttendanceApplication.ViewModels
{
    public class LoginViewModel
    {
        public int StudentId { get; set; }

        public string UtdId { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? MiddleInit { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Password { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
