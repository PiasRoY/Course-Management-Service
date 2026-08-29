using CourseManagement.Domain.Enums;
using System.Security.Claims;

namespace CourseManagement.Business.DTOs.BulkImportDTOs;

public class UserContextDto
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public IEnumerable<UserRoles> Roles { get; set; } = [];

    public UserContextDto(ClaimsPrincipal userClaims)
    {
        this.UserId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? "hangfire_unknown_userId";
        this.UserEmail = userClaims?.FindFirst(ClaimTypes.Email)?.Value
            ?? "hangfire_unknown_userEmail";
        this.Roles = userClaims?.FindAll(ClaimTypes.Role)?.Select(r => Enum.Parse<UserRoles>(r.Value)) ?? [];
    }
}
