using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class Enrollment : BaseAuditEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public Guid EnrolledBy { get; set; }
}
