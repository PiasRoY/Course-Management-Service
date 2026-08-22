using CourseManagement.Business.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = UserRoles.AdminOrStaff)]
public class EnrollmentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEnrollments()
    {
        return Ok(new { message = "List of enrollments" });
    }

    [HttpGet("{id}")]
    public IActionResult GetEnrollmentById(int id)
    {
        return Ok(new { message = $"Details of enrollment with ID: {id}" });
    }

    [HttpPost]
    public IActionResult CreateEnrollment()
    {
        return Ok(new { message = "Enrollment created successfully" });
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateEnrollment(int id)
    {
        return Ok(new { message = "Enrollment updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEnrollment(int id)
    {
        return Ok(new { message = "Enrollment deleted successfully" });
    }
}
