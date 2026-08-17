namespace CourseManagement.Domain.Entities;

public class CourseClass
{
    public Guid CourseId { get; set; }
    required public Course Course { get; set; }
    
    public Guid ClassId { get; set; }
    required public Class Class { get; set; }

    public Guid Instructor { get; set; }
}
