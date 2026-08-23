using CourseManagement.Business.Constants;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService authService;

    public AccountController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var userDto = await this.authService.CreateUserAsync(createUserRequest);
        return Ok(userDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDto>> Login(AuthenticateUserRequest authUserRequest, CancellationToken cancellationToken)
    {
        var tokenDto = await this.authService.AuthenticateUserAsync(authUserRequest);
        return Ok(tokenDto);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDto>> RefreshToken(TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var userId = this.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var tokenDto = await this.authService.RefreshAsync(tokenRequest, userId);
        return Ok(tokenDto);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var tokenEmail = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;

        if (changePasswordRequest.Email != tokenEmail)
        {
            return BadRequest(new { message = "Email does not match between the requested email and the token email." });
        }

        await this.authService.ChangePasswordAsync(changePasswordRequest);
        return Ok(new { message = "Password changed successfully." } );
    }

    [HttpPatch("update-user")]
    public async Task<ActionResult> UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        var userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var affectedRows = await this.authService.UpdateUserAsync(updateUserRequest, userId);

        if (affectedRows == 0)
        {
            return NotFound(new { message = "User not found. " });
        }

        return Ok(new
        {
            message = $"User has been updated successfully."
        });
    }

    [HttpDelete("delete-user")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult> DeleteUserAsync(DeleteUserRequest deleteUserRequest)
    {
        if (deleteUserRequest.UserId is null & string.IsNullOrEmpty(deleteUserRequest.Email))
        {
            return BadRequest(new { message = "Either UserId or Email must be provided." });
        }

        var affectedRows = await this.authService.DeleteAsync(deleteUserRequest);

        if (affectedRows == 0)
        {
            return NotFound(new { message = "User Not Found." });
        }

        return Ok(new { 
            message = $"User ([{deleteUserRequest.UserId}]/[{deleteUserRequest.Email}]) has been deleted successfully."
        });
    }

    [HttpPost("revoke-refresh-tokens")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult> RevokeRefreshTokens([FromBody] Guid userId)
    {
        await this.authService.RevokeRefreshTokensByUser(userId);

        return NoContent();
    }
}
