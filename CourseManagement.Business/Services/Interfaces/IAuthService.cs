using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.UserDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IAuthService
{
    Task<PageResult<UserDto>> GetUsersAsync(PaginationParams @params, CancellationToken cancellationToken);
    Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    public Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken,
        IEnumerable<string>? roles = null);
    public Task<TokenDto> AuthenticateUserAsync(AuthenticateUserRequest authenticateUserRequest,
        CancellationToken cancellationToken);
    public Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);
    public Task<TokenDto> RefreshAsync(TokenRequest tokenRequest, CancellationToken cancellationToken);
    Task<int> DeleteAsync(DeleteUserRequest deleteUserRequest, CancellationToken cancellationToken);
    Task<int> UpdateUserAsync(Guid userId, UpdateUserRequest updateUserRequest, CancellationToken cancellationToken);
    Task RevokeRefreshTokensByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task ChangeRolesAsync(ChangeRolesRequest changeRolesRequest, CancellationToken cancellationToken);
}
