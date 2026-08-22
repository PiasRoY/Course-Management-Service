using CourseManagement.Business.Options;

namespace CourseManagement.API.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));

        return services;
    }
}
