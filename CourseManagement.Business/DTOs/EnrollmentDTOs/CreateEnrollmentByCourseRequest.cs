using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class CreateEnrollmentByCourseRequest
{
    [Required(ErrorMessage = "StudentId is required.")]
    public required Guid StudentId { get; set; }

    [Required(ErrorMessage = "CourseId is required.")]
    public required Guid CourseId { get; set; }
}
