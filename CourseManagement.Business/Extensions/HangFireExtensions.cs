using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagement.Business.Extensions;

public static class HangFireExtensions
{
    public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(options => {
                options.UseNpgsqlConnection(configuration.GetConnectionString("PostgresConnection"));
            });
        });

        return services;
    }
}
