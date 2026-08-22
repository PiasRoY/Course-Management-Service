using CourseManagement.Business.Services;
using CourseManagement.Business.Services.Interfaces;

namespace CourseManagement.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddTransient<DbSeeder>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
