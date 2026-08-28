using CourseManagement.Business.Services;
using CourseManagement.Business.Services.Helpers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Common;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManagement.Business.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        services.AddScoped<ICsvFileHelper, CsvFileHelper>();
        services.AddScoped<IStorageService, StorageService>();

        services.AddScoped<IBulkService, BulkService>();
        services.AddScoped<ITaskManager, TaskManager>();
        services.AddScoped<IJobEventService, JobEventService>();

        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }
}
