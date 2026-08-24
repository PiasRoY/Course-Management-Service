using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.API;

public class DbSeeder
{
    private readonly IAuthService authService;
    private readonly ApplicationDbContext dbContext;

    public DbSeeder(
        IAuthService authService,
        ApplicationDbContext dbContext)
    {
        this.authService = authService;
        this.dbContext = dbContext;
    }

    public async Task DbSeed()
    {
        await this.dbContext.Database.MigrateAsync();
        await this.SeedRoles();
        await this.SeedAdminUsers();
        await this.SeedInstructorUsers();
    }

    private async Task SeedRoles()
    {
        if (await this.dbContext.UserRoles.AnyAsync())
        {
            return;
        }

        var roles = new List<UserRole>
        {
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Admin.ToString()
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Staff.ToString()
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Instructor.ToString()
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Student.ToString()
            }
        };

        await this.dbContext.UserRoles.AddRangeAsync(roles);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task SeedAdminUsers()
    {
        var createUser = new CreateUserRequest
        {
            FirstName = "Pias",
            LastName = "Roy",
            EmailAddress = "pias.roy@admin.com",
            Password = "password12345678"
        };

        if (await this.dbContext.Users.AnyAsync(u => u.EmailAddress == createUser.EmailAddress))
        {
            return;
        }

        await this.authService.CreateUserAsync(createUser, CancellationToken.None, [UserRoles.Admin.ToString()]);
    }

    private async Task SeedInstructorUsers()
    {
        var createUser = new CreateUserRequest
        {
            FirstName = "Pias",
            LastName = "Roy",
            EmailAddress = "pias@instructor.com",
            Password = "password12345678"
        };

        if (await this.dbContext.Users.AnyAsync(u => u.EmailAddress == createUser.EmailAddress))
        {
            return;
        }

        await this.authService.CreateUserAsync(createUser, CancellationToken.None, [UserRoles.Instructor.ToString()]);
    }
}
