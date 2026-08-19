using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Course : BaseAuditEntity
{
    public Guid CourseId { get; set; }
    required public string Name { get; set; }
    required public string Title { get; set; }
    required public int Credits { get; set; } = 3;

    public ICollection<CourseClass> CourseClasses { get; set; } = [];
}
