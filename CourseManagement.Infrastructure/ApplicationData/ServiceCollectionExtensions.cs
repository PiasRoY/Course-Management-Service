using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagement.Infrastructure.ApplicationData;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEFServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PostgresConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("CourseManagement.Infrastructure");
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ApplicationDbContext.DefaultSchema);
                }));

        return services;
    }
}
