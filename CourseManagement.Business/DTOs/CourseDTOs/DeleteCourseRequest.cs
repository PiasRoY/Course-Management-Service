using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.CourseDTOs;

public class DeleteCourseRequest
{
    [Required(ErrorMessage = "Course id is required.")]
    public required Guid CourseId { get; set; }
}
