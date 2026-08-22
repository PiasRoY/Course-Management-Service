using CourseManagement.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Domain.Entities;

public class User : BaseAuditEntity
{
    required public Guid UserId { get; set; }
    required public string EmailAddress { get; set; }
    required public string PasswordHash { get; set; }
    required public string FirstName { get; set; }
    required public string LastName { get; set; }
    required public ICollection<UserUserRole> UserUserRoles { get; set; }
}
