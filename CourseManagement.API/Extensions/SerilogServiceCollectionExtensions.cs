using Serilog;

namespace CourseManagement.API.Extensions;

public static class SerilogServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

        services.AddSerilog();

        return services;
    }
}
