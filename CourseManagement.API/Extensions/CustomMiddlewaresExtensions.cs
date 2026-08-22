using CourseManagement.API.Handlers;

namespace CourseManagement.API.Extensions;

public static class CustomMiddlewaresExtensions
{
    public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<SerilogLogContextMiddleware>();

        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();

        return services;
    }
}
