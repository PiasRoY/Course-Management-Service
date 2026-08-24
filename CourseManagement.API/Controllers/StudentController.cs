using CourseManagement.Business.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class StudentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetStudents()
    {
        return Ok(new { message = "List of students" });
    }

    [HttpGet("{id}")]
    public IActionResult GetStudentById(int id)
    {
        return Ok(new { message = $"Details of student with ID: {id}" });
    }

    [HttpPost]
    public IActionResult CreateStudent()
    {
        return Ok(new { message = "Student created successfully" });
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateStudent(int id)
    {
        return Ok(new { message = "Student updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStudent(int id)
    {
        return Ok(new { message = "Student deleted successfully" });
    }
}
