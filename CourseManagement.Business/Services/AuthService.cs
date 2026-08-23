using CourseManagement.Business.Constants;
using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Mappers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CourseManagement.Business.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> logger;
    private readonly ApplicationDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenService tokenService;

    public AuthService(
        ILogger<AuthService> logger,
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken, IEnumerable<string>? roles = null)
    {
        roles ??= [UserRoles.Student];

        var userExist = await this.dbContext
            .Users.AsNoTracking()
            .AnyAsync(u => u.EmailAddress == createUserRequest.EmailAddress, cancellationToken);

        if (userExist)
        {
            throw new InvalidOperationException($"A user with `{createUserRequest.EmailAddress}` already exists.");
        }

        this.logger.LogInformation("Creating a new user with email: {Email}", createUserRequest.EmailAddress);

        var useruserroles = await this.dbContext
            .UserRoles
            .Where(ur => roles.Contains(ur.RoleName))
            .Select(ur => new UserUserRole { UserRole = ur })
            .ToListAsync(cancellationToken);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = createUserRequest.EmailAddress,
            PasswordHash = this.passwordHasher.HashPassword(createUserRequest.Password),
            FirstName = createUserRequest.FirstName,
            LastName = createUserRequest.LastName,
            UserUserRoles = useruserroles
        };

        await this.dbContext.Users.AddAsync(user, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Created a new user with email: {Email}", createUserRequest.EmailAddress);

        return UserMapping.MapsToUserDto(user);
    }

    public async Task<TokenDto> AuthenticateUserAsync(AuthenticateUserRequest authenticateUserRequest, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == authenticateUserRequest.Email, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException("User not found.");
        }

        if (!this.passwordHasher.VerifyPassword(authenticateUserRequest.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid password.");
        }

        this.logger.LogInformation("Authenticated user : {Email}", authenticateUserRequest.Email);

        return await this.tokenService.GenerateTokensAsync(
            await this.CreateClaimsFromUserAsync(user, cancellationToken),
            cancellationToken);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
            .Users
            .FirstOrDefaultAsync(u => u.EmailAddress == changePasswordRequest.Email, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException("User not found.");
        }

        if (!this.passwordHasher.VerifyPassword(changePasswordRequest.OldPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid old password.");
        }

        user.PasswordHash = this.passwordHasher.HashPassword(changePasswordRequest.NewPassword);

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Password has been changed for user {Email}", user.EmailAddress);
    }

    public async Task<TokenDto> RefreshAsync(TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var claimsPrincipal = await tokenService.ExtractClaimsPrincipalFromTokenAsync(tokenRequest.ExpiredAccessToken, cancellationToken);
        return await this.tokenService.GenerateTokensAsync(claimsPrincipal.Claims, cancellationToken, tokenRequest.CurrentRefreshToken);
    }

    public async Task<int> UpdateUserAsync(UpdateUserRequest updateUserRequest, string userId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            throw new ArgumentException("UserId is not a valid GUID.", nameof(userId));
        }

        return await this.dbContext.Users
            .Where(u => u.UserId == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.FirstName, updateUserRequest.FirstName)
                .SetProperty(u => u.LastName, updateUserRequest.LastName)
                .SetProperty(u => u.UpdatedBy, id)
                .SetProperty(u => u.UpdatedAt, DateTime.UtcNow)
            );
    }

    public async Task<int> DeleteAsync(DeleteUserRequest deleteUserRequest, CancellationToken cancellationToken)
    {
        return await this.dbContext.Users
            .Where(
                u => deleteUserRequest.UserId != null ?
                u.UserId == deleteUserRequest.UserId :
                u.EmailAddress == deleteUserRequest.Email)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task RevokeRefreshTokensByUser(Guid userId, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
            .Users.AsNoTracking()
            .AnyAsync(u => u.UserId == userId, cancellationToken);

        if (!user)
        {
            throw new InvalidOperationException("User not found.");
        }

        await this.tokenService.RevokeAllRefreshTokensByUserAsync(userId, cancellationToken);
    }

    private async Task<List<Claim>> CreateClaimsFromUserAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new (ClaimTypes.Email, user.EmailAddress)
        };

        var roles = await this.dbContext
            .Users.AsNoTracking()
            .Where(u => u.UserId == user.UserId)
            .SelectMany(u => u.UserUserRoles.Select(uur => uur.UserRole))
            .ToListAsync(cancellationToken);


        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role.RoleName));
        }

        return claims;
    }
}
