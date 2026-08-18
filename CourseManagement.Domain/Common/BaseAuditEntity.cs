namespace CourseManagement.Domain.Common;

public abstract class BaseAuditEntity
{
    required public Guid CreatedBy { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111"); // SYSTEM ID
    required public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
