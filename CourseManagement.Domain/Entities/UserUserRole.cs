using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class UserUserRole : BaseAuditEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public UserRole UserRole { get; set; } = null!;
}
