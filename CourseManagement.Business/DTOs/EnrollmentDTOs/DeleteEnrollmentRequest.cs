using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class DeleteEnrollmentRequest
{
    [Required(ErrorMessage = "EnrollmentId is required.")]
    public Guid EnrollmentId { get; set; }
}
