using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Enrollment : BaseAuditEntity
{
    public Guid EnrollmentId { get; set; }
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public Guid? CourseId { get; set; }
    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Course? Course { get; set; } = null;
}
