using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Course : BaseAuditEntity
{
    public Guid Id { get; set; }
    required public string Name { get; set; }
    required public string Description { get; set; }
    public ICollection<CourseClass> CourseClasses { get; set; } = [];
}
