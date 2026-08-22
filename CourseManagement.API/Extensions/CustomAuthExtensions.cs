using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
