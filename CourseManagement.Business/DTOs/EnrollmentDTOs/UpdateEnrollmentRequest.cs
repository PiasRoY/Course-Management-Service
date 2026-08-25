namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class UpdateEnrollmentRequest
{
    public Guid? StudentId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? CourseId { get; set; }
}
