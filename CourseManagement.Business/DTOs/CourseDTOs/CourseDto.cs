namespace CourseManagement.Business.DTOs.CourseDTOs;

public class CourseDto
{
    required public Guid CourseId { get; set; }
    required public string Name { get; set; }
    required public string Title { get; set; }
    required public int Credits { get; set; }
    required public IEnumerable<string> ClassNames { get; set; }
}
