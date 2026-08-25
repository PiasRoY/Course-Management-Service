using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class AuthenticateUserRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address is invalid.")]
    required public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    required public string Password { get; set; }
}
