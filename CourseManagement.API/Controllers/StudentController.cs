using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
public class StudentController : ControllerBase
{
    private readonly IStudentService studentService;

    public StudentController(IStudentService studentService)
    {
        this.studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<StudentDto>>> GetStudents([FromQuery] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.GetStudentsAsync(@params, cancellationToken));
    }

    [HttpGet("{studentId}")]
    public async Task<ActionResult<StudentDto>> GetStudentById(Guid studentId, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.GetStudentByIdAsync(studentId, cancellationToken));
    }

    [HttpGet("roll-number/{rollNumber}")]
    public async Task<ActionResult<StudentDto>> GetStudentByRollNumberAsync(string rollNumber, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.GetStudentByRollNoAsync(rollNumber, cancellationToken));
    }

    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByStudent(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("Token is not valid.");
        }

        return Ok(await this.studentService.GetClassesByStudent(userId.Value, cancellationToken));
    }

    [HttpGet("courses")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByStudent(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("Token is not valid.");
        }

        return Ok(await this.studentService.GetCoursesByStudent(userId.Value, cancellationToken));
    }

    [HttpGet("classmates")]
    public async Task<ActionResult<PageResult<StudentDto>>> GetClassMatesByStudent(PaginationParams @params, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("Token is not valid");
        }

        return Ok(await this.studentService.GetClassMatesByStudent(userId.Value, @params, cancellationToken));
    }

    [HttpGet("coursemates")]
    public async Task<ActionResult<PageResult<StudentDto>>> GetCourseMatesByStudent(PaginationParams @params, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("Token is not valid");
        }

        return Ok(await this.studentService.GetCourseMatesByStudent(userId.Value, @params, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudentAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken)
    {
        var studentDto = await this.studentService.CreateStudentAsync(createStudentRequest, cancellationToken);
        return CreatedAtAction(
            nameof(GetStudentById),
            new { studentDto.StudentId },
            studentDto);
    }

    [HttpPatch("{studentId}")]
    public async Task<ActionResult<StudentDto>> UpdateStudentAsync(Guid studentId, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.UpdateStudentByIdAsync(studentId, updateStudentRequest, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStudentAsync(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken)
    {
        await this.studentService.DeleteStudentAsync(deleteStudentRequest, cancellationToken);
        return NoContent();
    }

    private Guid? GetUserId()
    {
        var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.Name);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return null;
        }

        return userId;
    }
}
