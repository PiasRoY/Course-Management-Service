using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Course : BaseAuditEntity
{
    public Guid CourseId { get; set; }
    required public string Code { get; set; }
    required public string Title { get; set; }
    required public int Credits { get; set; } = 3;

    public ICollection<Class> Classes { get; set; } = [];
}
