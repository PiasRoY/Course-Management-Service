using CourseManagement.Business.Constants;
using CourseManagement.Infrastructure.ApplicationData;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class CreateEnrollmentByClassNames
{
    [Required(ErrorMessage = "StudentRollNumber is required.")]
    [RegularExpression(RegexConstants.StudentRollNumberRegex, ErrorMessage = "Roll number is invalid.")]
    public required string StudentRollNumber { get; set; }

    [Required(ErrorMessage = "ClassName is required.")]
    [RegularExpression(DbConstants.AlphaNumericRegex, ErrorMessage = "Class name is invalid.")]
    public required string ClassName { get; set; }

    public string? CourseName { get; set; }
}
