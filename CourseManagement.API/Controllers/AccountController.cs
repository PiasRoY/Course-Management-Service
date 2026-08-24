using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var userDto = await this.authService.CreateUserAsync(createUserRequest, cancellationToken);
        return CreatedAtAction(nameof(Register), userDto);
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
        var tokenEmail = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;

        if (changePasswordRequest.Email != tokenEmail)
        {
            return BadRequest(new { message = "Email does not match between the requested email and the token email." });
        }

        await this.authService.ChangePasswordAsync(changePasswordRequest, cancellationToken);

        return NoContent();
    }

    [HttpPatch("update-user")]
    public async Task<ActionResult> UpdateUserAsync(UpdateUserRequest updateUserRequest, CancellationToken cancellationToken)
    {
        var userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var affectedRows = await this.authService.UpdateUserAsync(updateUserRequest, userId, cancellationToken);

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
    [Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
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
