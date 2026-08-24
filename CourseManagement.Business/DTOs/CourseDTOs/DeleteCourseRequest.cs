using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.CourseDTOs;

public class DeleteCourseRequest
{
    [Required(ErrorMessage = "Course name is required.")]
    public required string Name { get; set; }
}
