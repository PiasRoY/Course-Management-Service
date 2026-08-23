using CourseManagement.Business.DTOs.UserDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken,
        IEnumerable<string>? roles = null);
    public Task<TokenDto> AuthenticateUserAsync(AuthenticateUserRequest authenticateUserRequest,
        CancellationToken cancellationToken);
    public Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);
    public Task<TokenDto> RefreshAsync(TokenRequest tokenRequest, CancellationToken cancellationToken);
    Task<int> DeleteAsync(DeleteUserRequest deleteUserRequest, CancellationToken cancellationToken);
    Task<int> UpdateUserAsync(UpdateUserRequest updateUserRequest, string userId, CancellationToken cancellationToken);
    Task RevokeRefreshTokensByUser(Guid userId, CancellationToken cancellationToken);
}
