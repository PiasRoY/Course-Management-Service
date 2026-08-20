using CourseManagement.Business.Constants;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_Management_Service.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = UserRoles.AdminOrStaff)]
public class AccountController : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return Ok(new { message = "User has been registered successfully." });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return Ok(new { message = "User has logged in successfully." });
    }

    [HttpPost("refresh_token")]
    [AllowAnonymous]
    public IActionResult RefreshToken()
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("change-password")]
    public IActionResult ChangePassword()
    {
        throw new NotImplementedException();
    }
}
