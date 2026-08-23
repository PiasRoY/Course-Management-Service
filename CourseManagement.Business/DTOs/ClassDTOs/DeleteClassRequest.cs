using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class DeleteClassRequest
{
    [Required(ErrorMessage = "Class name is required.")]
    public required string Name { get; set; }
}
