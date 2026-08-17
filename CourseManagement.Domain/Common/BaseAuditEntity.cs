namespace CourseManagement.Domain.Common;

public class BaseAuditEntity
{
    required public Guid CreatedBy { get; set; }
    required public DateTime CreatedAt { get; set; }
    required public Guid UpdateBy { get; set; }
    required public DateTime UpdatedAt { get; set; }
}
