using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class UserRole : BaseAuditEntity
{
    public Guid RoleId { get; set; }
    public required string RoleName { get; set; }
}