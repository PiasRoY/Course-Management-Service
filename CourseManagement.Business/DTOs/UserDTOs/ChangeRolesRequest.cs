using CourseManagement.Business.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class ChangeRolesRequest
{
    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address is invalid.")]
    public required string UserEmail { get; set; }
    public IEnumerable<UserRoles> Roles { get; set; } = [];
}
