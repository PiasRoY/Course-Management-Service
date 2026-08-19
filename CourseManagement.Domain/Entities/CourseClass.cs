using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class CourseClass : BaseAuditEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}
