using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private const string RefreshTokenCookieName = "RefreshToken";
    
    private readonly IAuthService authService;

    public AccountController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpGet]
    [Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
    public async Task<ActionResult<PageResult<UserDto>>> GetUsersAsync([FromQuery] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.authService.GetUsersAsync(@params, cancellationToken));
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
    public async Task<ActionResult<PageResult<UserDto>>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await this.authService.GetUserByIdAsync(id, cancellationToken));
    }

    [HttpGet("email/{email}")]
    [Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
    public async Task<ActionResult<PageResult<UserDto>>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Ok(await this.authService.GetUserByEmailAsync(email, cancellationToken));
    }

    [HttpPost("register")]
    [Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
    public async Task<ActionResult<UserDto>> Register(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var userDto = await this.authService.CreateUserAsync(createUserRequest, cancellationToken);
        return CreatedAtAction(
                    nameof(GetUserById), 
                    new { id = userDto.UserId },
                    userDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(AuthenticateUserRequest authUserRequest, CancellationToken cancellationToken)
    {
        var tokenDto = await this.authService.AuthenticateUserAsync(authUserRequest, cancellationToken);
        
        SetRefreshToken(tokenDto.RefreshToken, tokenDto.RereshTokenExpiredAt);        
        
        return Ok(new { tokenDto.AccessToken });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var accessToken = ExtractBearerTokenFromAuthHeader();
        var refreshToken = GetRefreshToken();

        var tokenRequest = new TokenRequest
        {
            ExpiredAccessToken = accessToken,
            CurrentRefreshToken = refreshToken
        };

        var tokenDto = await this.authService.RefreshAsync(tokenRequest, cancellationToken);
        
        SetRefreshToken(tokenDto.RefreshToken, tokenDto.RereshTokenExpiredAt);        
        
        return Ok(new { tokenDto.AccessToken });
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var tokenEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

        if (changePasswordRequest.Email != tokenEmail)
        {
            return Unauthorized("Email does not match between the requested email and the token email.");
        }

        await this.authService.ChangePasswordAsync(changePasswordRequest, cancellationToken);

        return NoContent();
    }

    [HttpPatch("update-user")]
    public async Task<ActionResult> UpdateUserAsync(UpdateUserRequest updateUserRequest, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("Token must have valid user id.");
        }

        var affectedRows = await this.authService.UpdateUserAsync(userId, updateUserRequest, cancellationToken);

        if (affectedRows == 0)
        {
            return NotFound(new { message = "User not found." });
        }

        return NoContent();
    }

    [HttpDelete("delete-user")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<ActionResult> DeleteUserAsync(DeleteUserRequest deleteUserRequest, CancellationToken cancellationToken)
    {
        if (deleteUserRequest.UserId is null & string.IsNullOrEmpty(deleteUserRequest.Email))
        {
            return BadRequest(new { message = "Either UserId or Email must be provided." });
        }

        var affectedRows = await this.authService.DeleteAsync(deleteUserRequest, cancellationToken);

        if (affectedRows == 0)
        {
            return NotFound(new { message = "User Not Found." });
        }

        return NoContent();
    }

    [HttpPost("revoke-refresh-tokens")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<ActionResult> RevokeRefreshTokens([FromBody] Guid userId, CancellationToken cancellationToken)
    {
        await this.authService.RevokeRefreshTokensByUserAsync(userId, cancellationToken);

        return NoContent();
    }

    [HttpPost("change-roles")]
    [Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
    public async Task<IActionResult> ChangeRolesAsync(ChangeRolesRequest changeRolesRequest, CancellationToken cancellationToken)
    {
        await this.authService.ChangeRolesAsync(changeRolesRequest, cancellationToken);
        return NoContent();
    }
    
    private string ExtractBearerTokenFromAuthHeader()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            throw new InvalidOperationException("Bearer token not provided.");
        }
        
        return authHeader.Substring("Bearer ".Length).Trim();
    }

    private string GetRefreshToken()
    {
        Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken);

        return string.IsNullOrEmpty(refreshToken) ? 
            throw new InvalidOperationException("Refresh token not provided.") : 
            refreshToken;
    }

    private void SetRefreshToken(string refreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = Url.Action(nameof(RefreshToken))
        });
    }
}
