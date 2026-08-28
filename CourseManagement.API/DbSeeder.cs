using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        await this.SeedUsers();
    }

    private async Task SeedRoles()
    {
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Seeds", "Roles.json");
        var seedFile = await File.ReadAllTextAsync(seedFilePath);
        var seedRoles = JsonSerializer.Deserialize<List<SeedRole>>(seedFile, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }})
            ?? throw new InvalidOperationException($"Unable to deserialize seed roles from `{seedFilePath}`.");

        var roles = new List<UserRole>();

        foreach (var role in seedRoles)
        {
            var isExists = await this.dbContext.UserRoles.AnyAsync(r => r.RoleName == role.RoleName.ToString());
            if (role == null || isExists)
            {
                continue;
            }

            roles.Add(new UserRole
            {
                RoleId = Guid.NewGuid(),
                RoleName = role.ToString(),
            }); 
        }

        await this.dbContext.UserRoles.AddRangeAsync(roles);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task SeedUsers()
    {
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Seeds", "Users.json");
        var seedFile = await File.ReadAllTextAsync(seedFilePath);
        var seedUsers = JsonSerializer.Deserialize<List<SeedUser>>(seedFile, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } })
            ?? throw new InvalidOperationException($"Unable to deserialize seed users from `{seedFilePath}`.");

        foreach (var seedUser in seedUsers)
        {
            var createUser = new CreateUserRequest
            {
                FirstName = seedUser.FirstName,
                LastName = seedUser.LastName,
                EmailAddress = seedUser.EmailAddress,
                Password = seedUser.Password,
                Roles = seedUser.Roles
            };

            var isExists = await this.dbContext.Users.AnyAsync(u => u.EmailAddress == createUser.EmailAddress);
            
            if (!isExists)
            {
                await this.authService.CreateUserAsync(createUser, CancellationToken.None);
            }
        }
    }

    private sealed class SeedUser
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string EmailAddress { get; init; }
        public required string Password { get; init; }
        public List<UserRoles> Roles { get; init; } = [];
    }

    private sealed class SeedRole
    {
        public required UserRoles RoleName { get; set; }
    }
}
