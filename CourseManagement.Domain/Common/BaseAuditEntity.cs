namespace CourseManagement.Domain.Common;

public abstract class BaseAuditEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; } = null;
    public DateTime? UpdatedAt { get; set; } = null;
}
