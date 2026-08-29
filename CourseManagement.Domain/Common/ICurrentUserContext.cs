using CourseManagement.Domain.Enums;

namespace CourseManagement.Domain.Common;

public interface ICurrentUserContext
{
    public string? UserId { get; }
    public string? Email { get; }
    public IEnumerable<UserRoles>? Roles { get; }
    void SetUserContext(string? userId, string? email, IEnumerable<UserRoles>? roles);
}
