using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
