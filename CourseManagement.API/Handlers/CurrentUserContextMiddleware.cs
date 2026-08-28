using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;
using System.Security.Claims;

namespace CourseManagement.API.Handlers;

public class CurrentUserContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        var userClaims = httpContext.User;

        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = userClaims.FindFirst(ClaimTypes.Email)?.Value;
        var roles = userClaims.FindAll(ClaimTypes.Role).Select(r => Enum.Parse<UserRoles>(r.Value));

        var currentUserContext = httpContext.RequestServices.GetRequiredService<ICurrentUserContext>();
        currentUserContext.SetUserContext(userId, email, roles);

        await next(httpContext);
    }
}
