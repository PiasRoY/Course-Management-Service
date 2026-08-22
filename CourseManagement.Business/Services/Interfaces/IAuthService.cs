using CourseManagement.Business.DTOs.UserDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, IEnumerable<string>? roles = null);
    public Task<TokenDto> AuthenticateUserAsync(AuthenticateUserRequest authenticateUserRequest);
    public Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    public Task<TokenDto> RefreshAsync(TokenRequest tokenRequest);
}
