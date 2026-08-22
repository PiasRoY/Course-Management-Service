using CourseManagement.Business.Constants;
using CourseManagement.Business.DTOs.UserDTOs;
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
                RoleName = UserRoles.Admin
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Staff
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Instructor
            },
            new()
            {
                RoleId = Guid.NewGuid(),
                RoleName = UserRoles.Student
            }
        };

        await this.dbContext.UserRoles.AddRangeAsync(roles);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task SeedAdminUsers()
    {
        if (await this.dbContext.Users.AnyAsync())
        {
            return;
        }

        var createUser = new CreateUserRequest
        {
            FirstName = "Pias",
            LastName = "Roy",
            EmailAddress = "pias.roy@admin.com",
            Password = "password12345678"
        };

        await this.authService.CreateUserAsync(createUser, [UserRoles.Admin]);
    }
}
