using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class ChangeRolesRequest
{
    public required Guid UserId { get; set; }
    public IEnumerable<UserRoles> Roles { get; set; } = [];
}
