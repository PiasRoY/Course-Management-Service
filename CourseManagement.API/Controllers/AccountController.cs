using CourseManagement.Business.Constants;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var tokenDto = await this.authService.RefreshAsync(tokenRequest);
        return Ok(tokenDto);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        await this.authService.ChangePasswordAsync(changePasswordRequest);
        return Ok(new { message = "Password changed successfully." } );
    }

    // UpdateUserAsync
    // DeleteUserAsync
}
