using CourseManagement.Infrastructure.ApplicationData;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.CourseDTOs;

public class CreateCourseRequest
{
    [Required(ErrorMessage = "Course Name is required.")]
    [RegularExpression(DbConstants.AlphaNumericRegex, ErrorMessage = "Course name is invalid.")]
    required public string Name { get; set; }

    [Required(ErrorMessage = "Course Title is required.")]
    required public string Title { get; set; }

    [Required(ErrorMessage = "Course Credits are required.")]
    required public int Credits { get; set; }

    [Required(ErrorMessage = "Course classes are required.")]
    required public IEnumerable<string> ClassNames { get; set; }
}
