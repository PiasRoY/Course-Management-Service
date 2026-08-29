namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class EnrollmentCourseDto
{
    public required Guid CourseId { get; set; }
    public required Guid StudentId { get; set; }
    public required string EnrolledBy { get; set; }
    public required DateTime EnrolledAt { get; set; }
}
