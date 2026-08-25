using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.DTOs.ClassDTOs;

public class ClassDto
{
    public required Guid ClassId { get; set; }
    public required string Name { get; set; }
    public required Semester Semester { get; set; }
    public required string Calendar { get; set; }
    public required string SectionCode { get; set; }
    public required string InstructorName { get; set; }
    public required string InstructorEmail { get; set; }
}
