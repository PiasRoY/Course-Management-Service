using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class AuthenticateUserRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must include a valid domain extension.")]
    required public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    required public string Password { get; set; }
}
