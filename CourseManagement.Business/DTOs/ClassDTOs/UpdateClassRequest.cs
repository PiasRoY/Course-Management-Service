using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class UpdateClassRequest
{
    public string? ClassName { get; set; }
    public Semester? Semester { get; set; }
    public int? Year { get; set; }
    public string? SectionCode { get; set; }

    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address is invalid.")]
    public string? InstructorEmail { get; set; }
}
