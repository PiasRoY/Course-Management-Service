using CourseManagement.Business.Enums;
using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagement.Business.Extensions;

public static class CustomAuthExtensions
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
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

            options.AddPolicy(nameof(UserPolicies.AdminOrStaff), policy =>
            {
                policy.RequireRole(nameof(UserRoles.Admin), nameof(UserRoles.Staff));
            });
        });

        return services;
    }
}
