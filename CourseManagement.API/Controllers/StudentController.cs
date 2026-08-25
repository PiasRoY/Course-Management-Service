using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class StudentController : ControllerBase
{
    private readonly IStudentService studentService;

    public StudentController(IStudentService studentService)
    {
        this.studentService = studentService;
    }

    [HttpGet]
    public IActionResult GetStudents()
    {
        throw new NotImplementedException(); // TODO: Pagination
    }

    [HttpGet("{number}")]
    public async Task<ActionResult<StudentDto>> GetStudentByRollNumberAsync(string rollNumber, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.GetStudentByRollNoAsync(rollNumber, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken)
    {
        return CreatedAtAction(nameof(CreateStudent),
                               await this.studentService.CreateStudentByRollNoAsync(createStudentRequest, cancellationToken));
    }

    [HttpPatch("{number}")]
    public async Task<ActionResult<StudentDto>> UpdateStudent(string rollNumber, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.studentService.UpdateStudentByRollNoAsync(rollNumber, updateStudentRequest, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStudent(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken)
    {
        await this.studentService.DeleteStudentAsync(deleteStudentRequest, cancellationToken);
        return NoContent();
    }
}
