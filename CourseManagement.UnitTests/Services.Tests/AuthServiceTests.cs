using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Services;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace CourseManagement.UnitTests.Services.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly Mock<IPasswordHasher> passwordHasher;
    private readonly UserRole studentRole;
    private readonly AuthService authService;
    private readonly Mock<ITokenService> tokenService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());
        this.studentRole = new UserRole
        {
            RoleId = Guid.NewGuid(),
            RoleName = UserRoles.Student.ToString()
        };

        this.dbContext.UserRoles.Add(this.studentRole);
        this.dbContext.SaveChanges();

        this.passwordHasher = new Mock<IPasswordHasher>();
        this.passwordHasher
            .Setup(hasher => hasher.HashPassword("Password123!"))
            .Returns("hashed-password");
        this.passwordHasher
            .Setup(hasher => hasher.VerifyPassword("Password123!", "hashed-password"))
            .Returns(true);

        this.tokenService = new Mock<ITokenService>();
        this.tokenService
            .Setup(ts => ts.GenerateTokensAsync(
                It.IsAny<IEnumerable<Claim>>(),
                CancellationToken.None,
                null))
            .ReturnsAsync(new TokenDto { AccessToken = "access_token", RefreshToken = "refresh_token", RereshTokenExpiredAt = DateTime.MaxValue });

        this.authService = new AuthService(
            Mock.Of<ILogger<AuthService>>(),
            this.dbContext,
            this.passwordHasher.Object,
            this.tokenService.Object);
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task CreateUserAsync_WithValidRequest_CreatesUserAndReturnsUserDto()
    {
        var request = new CreateUserRequest
        {
            EmailAddress = "student@example.com",
            Password = "Password123!",
            FirstName = "Jane",
            LastName = "Student"
        };

        var result = await this.authService.CreateUserAsync(request, CancellationToken.None);

        Assert.Equal("student@example.com", result.EmailAddress);
        Assert.Equal("Jane Student", result.FullName);

        var createdUser = await this.dbContext.Users
            .Include(user => user.UserUserRoles)
            .SingleAsync();

        Assert.Equal("hashed-password", createdUser.PasswordHash);
        Assert.Contains(createdUser.UserUserRoles, userRole => userRole.RoleId == this.studentRole.RoleId);
        this.passwordHasher.Verify(hasher => hasher.HashPassword("Password123!"), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        this.dbContext.Users.Add(new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        });
        await this.dbContext.SaveChangesAsync();

        var request = new CreateUserRequest
        {
            EmailAddress = "student@example.com",
            Password = "Password123!",
            FirstName = "Pias",
            LastName = "Student"
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => this.authService.CreateUserAsync(request, CancellationToken.None));

        Assert.Equal("A user with `student@example.com` already exists.", exception.Message);
        this.passwordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithValidRequest_ProvidesTokenDtoAsync()
    {
        this.dbContext.Users.Add(new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        });
        await this.dbContext.SaveChangesAsync();

        var authRequest = new AuthenticateUserRequest
        {
            Email = "student@example.com",
            Password = "Password123!"
        };

        var tokenDto = await this.authService.AuthenticateUserAsync(authRequest, CancellationToken.None);

        this.tokenService.Verify(tokenService => tokenService.GenerateTokensAsync(It.IsAny<IEnumerable<Claim>>(), CancellationToken.None, null), Times.Once);
        this.passwordHasher.Verify(hasher => hasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        Assert.Equal("access_token", tokenDto.AccessToken);
        Assert.Equal("refresh_token", tokenDto.RefreshToken);
        Assert.Equal(DateTime.MaxValue, tokenDto.RereshTokenExpiredAt);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithWrongPassword_ProvidesTokenDtoAsync()
    {
        this.dbContext.Users.Add(new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        });
        await this.dbContext.SaveChangesAsync();

        var authRequest = new AuthenticateUserRequest
        {
            Email = "student@example.com",
            Password = "Password123!_WRONG"
        };

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                        this.authService.AuthenticateUserAsync(authRequest, CancellationToken.None));

        Assert.Equal("Invalid password.", exception.Message);

        this.passwordHasher.Verify(hasher => hasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        this.tokenService.Verify(tokenService => tokenService.GenerateTokensAsync(It.IsAny<IEnumerable<Claim>>(), CancellationToken.None, null), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidRequest_ChangesPassword()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        };

        await this.dbContext.Users.AddAsync(user);
        await this.dbContext.SaveChangesAsync();

        this.passwordHasher
            .Setup(hasher => hasher.HashPassword("NewPassword123!"))
            .Returns("new-hashed-password");

        await this.authService.ChangePasswordAsync(new ChangePasswordRequest
        {
            Email = user.EmailAddress,
            OldPassword = "Password123!",
            NewPassword = "NewPassword123!"
        }, CancellationToken.None);

        var updatedUser = await this.dbContext.Users.AsNoTracking().SingleAsync();

        Assert.Equal("new-hashed-password", updatedUser.PasswordHash);
        
        this.passwordHasher.Verify(hasher => hasher.VerifyPassword("Password123!", "hashed-password"), Times.Once);
        this.passwordHasher.Verify(hasher => hasher.HashPassword("NewPassword123!"), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_WithValidRequest_ReturnsNewTokens()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };

        this.tokenService
            .Setup(tokenService => tokenService.ExtractClaimsPrincipalFromTokenAsync("expired-access-token", CancellationToken.None))
            .ReturnsAsync(new ClaimsPrincipal(new ClaimsIdentity(claims)));

        this.tokenService
            .Setup(tokenService => tokenService.GenerateTokensAsync(It.IsAny<IEnumerable<Claim>>(), CancellationToken.None, "current-refresh-token"))
            .ReturnsAsync(new TokenDto
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token",
                RereshTokenExpiredAt = DateTime.MaxValue
            });

        var result = await this.authService.RefreshAsync(new TokenRequest
        {
            ExpiredAccessToken = "expired-access-token",
            CurrentRefreshToken = "current-refresh-token"
        }, CancellationToken.None);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);

        this.tokenService.Verify(tokenService => tokenService.ExtractClaimsPrincipalFromTokenAsync("expired-access-token", CancellationToken.None), Times.Once);
        this.tokenService.Verify(tokenService => tokenService.GenerateTokensAsync(It.IsAny<IEnumerable<Claim>>(), CancellationToken.None, "current-refresh-token"), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokensByUserAsync_WithExistingUser_RevokesTokens()
    {
        var userId = Guid.NewGuid();
        this.dbContext.Users.Add(new User
        {
            UserId = userId,
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        });
        await this.dbContext.SaveChangesAsync();

        await this.authService.RevokeRefreshTokensByUserAsync(userId, CancellationToken.None);

        this.tokenService.Verify(tokenService => tokenService.RevokeAllRefreshTokensByUserAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ChangeRolesAsync_WithValidRequest_ReplacesUserRoles()
    {
        var instructorRole = new UserRole
        {
            RoleId = Guid.NewGuid(),
            RoleName = UserRoles.Instructor.ToString()
        };

        await this.dbContext.UserRoles.AddAsync(instructorRole);

        var userId = Guid.NewGuid();

        this.dbContext.Users.Add(new User
        {
            UserId = userId,
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = [new UserUserRole { UserRole = this.studentRole }]
        });

        await this.dbContext.SaveChangesAsync();

        await this.authService.ChangeRolesAsync(new ChangeRolesRequest
        {
            UserId = userId,
            Roles = [UserRoles.Instructor]
        }, CancellationToken.None);

        var updatedUser = await this.dbContext.Users
            .Include(user => user.UserUserRoles)
            .ThenInclude(userRole => userRole.UserRole)
            .SingleAsync();

        Assert.Single(updatedUser.UserUserRoles);
        Assert.Equal(UserRoles.Instructor.ToString(), updatedUser.UserUserRoles.Single().UserRole.RoleName);
    }
}
