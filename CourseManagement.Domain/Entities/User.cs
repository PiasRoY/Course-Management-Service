using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Domain.Entities;

public class User : BaseAuditEntity
{
    required public string Id { get; set; }
    required public string EmailAddress { get; set; }
    required public string Password { get; set; }
    required public string FirstName { get; set; }
    public string MiddleName { get; set; } = string.Empty;
    required public string LastName { get; set; }
    required public UserRole Role { get; set; }
}
