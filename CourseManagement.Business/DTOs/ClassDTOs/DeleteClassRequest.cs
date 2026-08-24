using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class DeleteClassRequest
{
    [Required(ErrorMessage = "Class name is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must include a valid domain extension.")]
    public required string Name { get; set; }
}
