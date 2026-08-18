using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Domain.Entities;

public class Class : BaseAuditEntity
{
    public Guid ClassId { get; set; }
    required public string Name { get; set; }
    required public Semester Semester { get; set; }
    required public int Year { get; set; }
    public string TermDisplay => $"{Semester} {Year}";
    required public string SectionCode { get; set; }
    public Guid InstructorId { get; set; }

    public User Instructor { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
}
