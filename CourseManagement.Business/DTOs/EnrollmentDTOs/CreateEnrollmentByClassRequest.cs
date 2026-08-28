using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class CreateEnrollmentByClassRequest
{
    [Required(ErrorMessage = "StudentId is required.")]
    public required Guid StudentId { get; set; }

    [Required(ErrorMessage = "ClassId is required.")]
    public required Guid ClassId { get; set; }
}
