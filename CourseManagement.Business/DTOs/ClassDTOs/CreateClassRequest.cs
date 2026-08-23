using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class CreateClassRequest
{
    [Required(ErrorMessage = "Class name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Semester name is required.")]
    public required Semester Semester { get; set; }

    [Required(ErrorMessage ="Year is required")]
    public required int Year { get; set; }

    [Required(ErrorMessage = "SectionCode is required.")]
    public required string SectionCode { get; set; }

    [Required(ErrorMessage = "Instructor email is required.")]
    public required string InstructorEmail { get; set; }
}
