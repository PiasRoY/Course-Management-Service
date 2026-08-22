using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace CourseManagement.API.Extensions;

public static class CustomAuthExtensions
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        var authSection = configuration.GetSection(nameof(AuthOptions));
        var authOptions = authSection.Get<AuthOptions>()
                    ?? throw new InvalidOperationException("AuthOptions section is missing or invalid.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenValidationParametersFactory.Create(authOptions);
            });

        services.AddAuthorization();

        return services;
    }
}
