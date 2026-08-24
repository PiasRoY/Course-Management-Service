namespace CourseManagement.Business.DTOs.CourseDTOs;

public class UpdateCourseRequest
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public int? Credits { get; set; }
    public IEnumerable<string>? ClassNames { get; set; }
}
