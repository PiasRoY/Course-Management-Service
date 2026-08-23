using CourseManagement.Business.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagement.Business.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services)
    {
        services
            .AddOptions<AuthOptions>()
            .BindConfiguration(nameof(AuthOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
