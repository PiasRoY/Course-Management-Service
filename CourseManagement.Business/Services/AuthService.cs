using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Extensions;
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

    public async Task<PageResult<UserDto>> GetUsersAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Users
                         .OrderBy(u => u.CreatedAt)
                         .ThenBy(u => u.UserId)
                         .Select(UserMapping.ProjectToUserDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userDto = await this.dbContext
                             .Users
                             .Select(UserMapping.ProjectToUserDto)
                             .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        return userDto ?? throw new UserNotFoundException(userId);
    }

    public async Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
                             .Users
                             .Select(UserMapping.ProjectToUserDto)
                             .SingleOrDefaultAsync(u => u.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);

        return user ?? throw new UserNotFoundException(email);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        if (createUserRequest.Roles.Count == 0)
        {
            createUserRequest.Roles.Add(UserRoles.Student);
        }

        var roles = createUserRequest.Roles.Select(r => r.ToString());

        var userExist = await this.dbContext
            .Users
            .AnyAsync(u => u.EmailAddress.Equals(createUserRequest.EmailAddress, StringComparison.OrdinalIgnoreCase), cancellationToken);

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
                             .Users
                             .AsNoTracking()
                             .SingleOrDefaultAsync(u => u.EmailAddress.Equals(authenticateUserRequest.Email, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(authenticateUserRequest.Email);
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
                             .SingleOrDefaultAsync(u => u.EmailAddress.Equals(changePasswordRequest.Email, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(changePasswordRequest.Email);
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

    public async Task<int> UpdateUserAsync(Guid userId, UpdateUserRequest updateUserRequest, CancellationToken cancellationToken)
    {
        var affectedRow = await this.dbContext.Users
                                              .Where(u => u.UserId == userId)
                                              .ExecuteUpdateAsync(s => s
                                                  .SetProperty(u => u.FirstName, updateUserRequest.FirstName)
                                                  .SetProperty(u => u.LastName, updateUserRequest.LastName)
                                                  .SetProperty(u => u.UpdatedBy, userId)
                                                  .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                                                  cancellationToken
                                              );

        this.logger.LogInformation("User with id {Id} has been updated", userId);

        return affectedRow;
    }

    public async Task<int> DeleteAsync(DeleteUserRequest deleteUserRequest, CancellationToken cancellationToken)
    {
        var affectedRow = await this.dbContext.Users
                                              .Where(
                                                  u => deleteUserRequest.UserId != null ?
                                                  u.UserId == deleteUserRequest.UserId :
                                                  u.EmailAddress.Equals(deleteUserRequest.Email, StringComparison.OrdinalIgnoreCase))
                                              .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("User with email {Email} OR id {Id} has been deleted.", deleteUserRequest.Email, deleteUserRequest.UserId);

        return affectedRow;
    }

    public async Task RevokeRefreshTokensByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
                             .Users
                             .AnyAsync(u => u.UserId == userId, cancellationToken);

        if (!user)
        {
            throw new InvalidOperationException("User not found.");
        }

        await this.tokenService.RevokeAllRefreshTokensByUserAsync(userId, cancellationToken);
    }

    public async Task ChangeRolesAsync(ChangeRolesRequest changeRolesRequest, CancellationToken cancellationToken)
    {
        var user = await this.dbContext
                             .Users
                             .Include(u => u.UserUserRoles)
                             .SingleOrDefaultAsync(u => u.UserId == changeRolesRequest.UserId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(changeRolesRequest.UserId);
        }

        user.UserUserRoles.Clear();

        var requestedRoles = changeRolesRequest.Roles.Select(r => r.ToString());

        var userRoleRoles = await this.dbContext
            .UserRoles
            .Where(ur => requestedRoles.Contains(ur.RoleName))
            .Select(ur => new UserUserRole
            {
                User = user,
                UserRole = ur
            })
            .ToListAsync(cancellationToken);

        user.UserUserRoles = userRoleRoles;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Roles for {UserEmail} are changed to {Roles}", changeRolesRequest.UserId, changeRolesRequest.Roles);
    }

    private async Task<List<Claim>> CreateClaimsFromUserAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new (ClaimTypes.Email, user.EmailAddress)
        };

        var roles = await this.dbContext
                              .Users
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
