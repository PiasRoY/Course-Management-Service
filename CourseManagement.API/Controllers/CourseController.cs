using CourseManagement.Business.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class CourseController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCourses()
    {
        return Ok(new { message = "List of courses" });
    }

    [HttpGet("{id}")]
    public IActionResult GetCourseById(int id)
    {
        return Ok(new { message = $"Details of course with ID: {id}" });
    }

    [HttpPost]
    public IActionResult CreateCourse()
    {
        return Ok(new { message = "Course created successfully" });
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateCourse(int id)
    {
        return Ok(new { message = "Course updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCourse(int id)
    {
        return Ok(new { message = "Course deleted successfully" });
    }
}