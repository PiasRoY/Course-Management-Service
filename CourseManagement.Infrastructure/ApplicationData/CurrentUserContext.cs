using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Infrastructure.ApplicationData;

public class CurrentUserContext : ICurrentUserContext
{
    public string? UserId { get; private set; }
    public string? Email { get; private set; }
    public IEnumerable<UserRoles> Roles { get; private set; } = [];

    public void SetUserContext(string? userId, string? email, IEnumerable<UserRoles>? roles)
    {
        UserId = userId;
        Email = email;
        Roles = roles ?? [];
    }
}