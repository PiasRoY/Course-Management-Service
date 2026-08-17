using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Class : BaseAuditEntity
{
    public Guid Id { get; set; }
    required public string Name { get; set; }
    public ICollection<CourseClass> CourseClasses { get; set; } = [];
}
