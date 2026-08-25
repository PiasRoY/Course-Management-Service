using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class CreateEnrollmentRequest
{
    [Required(ErrorMessage = "StudentId is required.")]
    public required Guid StudentId { get; set; }

    [Required(ErrorMessage = "ClassId is required.")]
    public required Guid ClassId { get; set; }

    [Required(ErrorMessage = "CourseId is required.")]
    public required Guid CourseId { get; set; }
}
