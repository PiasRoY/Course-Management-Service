using CourseManagement.Business.DTOs.UserDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, IEnumerable<string>? roles = null);
    public Task<TokenDto> AuthenticateUserAsync(AuthenticateUserRequest authenticateUserRequest);
    public Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    public Task<TokenDto> RefreshAsync(TokenRequest tokenRequest, string userId);
    Task<int> DeleteAsync(DeleteUserRequest deleteUserRequest);
    Task<int> UpdateUserAsync(UpdateUserRequest updateUserRequest, string userId);
    Task RevokeRefreshTokensByUser(Guid userId);
}
