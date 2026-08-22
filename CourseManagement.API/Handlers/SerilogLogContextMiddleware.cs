using Serilog.Context;
using System.Security.Claims;

namespace CourseManagement.API.Handlers;

public class SerilogLogContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        var user = httpContext.User;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown_userid";
        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "unknown_email";
        var roles = user.FindAll(ClaimTypes.Role);

        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserEmail", email))
        using (LogContext.PushProperty("UserRoles", roles))
        using (LogContext.PushProperty("RequestPath", httpContext.Request.Path))
        {
            await next(httpContext);
        }
    }
}
