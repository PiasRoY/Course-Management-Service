using CourseManagement.Infrastructure.ApplicationData;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.CourseDTOs;

public class UpdateCourseRequest
{
    [RegularExpression(DbConstants.AlphaNumericRegex, ErrorMessage = "Course name is invalid.")]
    public string? Name { get; set; }
    public string? Title { get; set; }
    public int? Credits { get; set; }
    public IEnumerable<string>? ClassNames { get; set; }
}
