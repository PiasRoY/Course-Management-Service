using CourseManagement.Business.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address is invalid.")]
    required public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    required public string Password { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    required public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    required public string LastName { get; set; }

    public ICollection<UserRoles> Roles { get; set; } = [];
}
