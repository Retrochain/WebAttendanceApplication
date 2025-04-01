using System.ComponentModel.DataAnnotations;

namespace WebAttendanceApplication.ViewModels
{
    public class SetPasswordViewModel
    {
        [StringLength(10, MinimumLength = 10, ErrorMessage = "UTD ID needs to be 10 digits long!")]
        public string utdID { get; set; } = null!;

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
