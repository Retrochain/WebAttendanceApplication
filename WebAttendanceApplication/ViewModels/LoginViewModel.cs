using System.ComponentModel.DataAnnotations;

namespace WebAttendanceApplication.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UtdId { get; set; } = null!;


        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
