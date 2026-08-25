using CourseManagement.Business.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog.Context;
using System.Security.Claims;

namespace CourseManagement.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        this.logger.LogError(exception, "Unhandled exception.");

        var (statusCode, title) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Not Authorized."),
            SecurityTokenException => (StatusCodes.Status401Unauthorized, "Token not authorized."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request."),
            NotFoundException => (StatusCodes.Status404NotFound, "Not found."),
            InvalidOperationException => (StatusCodes.Status409Conflict, "An invalid operation occured."),
            DbUpdateException => (StatusCodes.Status409Conflict, "Invalid db write query happened."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
