using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class CreateEnrollmentByClassNames
{
    [Required(ErrorMessage = "StudentRollNumber is required.")]
    public required string StudentRollNumber { get; set; }

    [Required(ErrorMessage = "ClassName is required.")]
    public required string ClassName { get; set; }

    public string? CourseName { get; set; }
}
