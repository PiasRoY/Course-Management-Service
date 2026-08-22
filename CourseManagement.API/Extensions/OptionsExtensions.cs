using CourseManagement.Business.Options;

namespace CourseManagement.API.Extensions;

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
