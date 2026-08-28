namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class EnrollmentDto
{
    public required Guid EnrollmentId { get; set; }
    public required Guid StudentId { get; set; }
    public string? StudentEmail { get; set; }
    public required Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }
    public required Guid EnrolledBy { get; set; }
    public required DateTime EnrolledAt { get; set; }
}
