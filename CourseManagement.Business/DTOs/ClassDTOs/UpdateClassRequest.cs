using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class UpdateClassRequest
{
    [Required(ErrorMessage = "Class name is required.")]
    public required string Name { get; set; }
    public Semester? Semester { get; set; }
    public int? Year { get; set; }
    public string? SectionCode { get; set; }
    public string? InstructorEmail { get; set; }
}
