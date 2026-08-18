using Microsoft.AspNetCore.Mvc;

namespace Course_Management_Service.Controllers;

[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    [HttpGet]
    public string Users()
    {
        return "Hello World!";
    }
}
