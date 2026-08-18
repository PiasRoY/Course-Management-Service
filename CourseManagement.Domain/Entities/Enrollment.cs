using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Enrollment : BaseAuditEntity
{
    public Guid EnrollmentId { get; set; }
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public Guid EnrolledById { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    required public User Student { get; set; }
    required public User EnrolledBy { get; set; }
    required public Class Class { get; set; }
}
