using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must include a valid domain extension.")]
        required public string Email { get; set; }

        [Required(ErrorMessage = "Old Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        required public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        required public string NewPassword { get; set; }
    }
}