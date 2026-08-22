using CourseManagement.Business.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = UserRoles.AdminOrStaff)]
public class ClassController : ControllerBase
{
    [HttpGet]
    public IActionResult GetClasses()
    {
        return Ok(new { message = "List of classes" });
    }

    [HttpGet("{id}")]
    public IActionResult GetClassById(int id)
    {
        return Ok(new { message = $"Details of class with ID: {id}" });
    }

    [HttpPost]
    public IActionResult CreateClass()
    {
        return Ok(new { message = "Class created successfully" });
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateClass(int id)
    {
        return Ok(new { message = "Class updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteClass(int id)
    {
        return Ok(new { message = "Class deleted successfully" });
    }
}
