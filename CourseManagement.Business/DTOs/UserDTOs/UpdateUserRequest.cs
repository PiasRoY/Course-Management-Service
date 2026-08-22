using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "First name is required.")]
    required public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    required public string LastName { get; set; }
}
