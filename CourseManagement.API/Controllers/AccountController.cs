using Microsoft.AspNetCore.Mvc;

namespace Course_Management_Service.Controllers;

[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> logger;

    public AccountController(ILogger<AccountController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    public string Users()
    {
        this.logger.LogInformation($"{nameof(Users)} method called.");
        return "Hello World!";
    }
}
