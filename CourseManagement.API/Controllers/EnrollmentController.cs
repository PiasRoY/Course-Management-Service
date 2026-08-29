using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        this.enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<EnrollmentDto>>> GetEnrollmentsAsync([FromQuery] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.GetEnrollmentsAsync(@params, cancellationToken));
    }

    [HttpGet("student-enrollment/{studentId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.GetEnrollmentsByStudentIdAsync(studentId, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.GetEnrollmentByIdAsync(id, cancellationToken));
    }

    [HttpPost("class")]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollmentByClassAsync(CreateEnrollmentByClassRequest request, CancellationToken cancellationToken)
    {
        var enrolledByEmail = GetUserEmail();

        if (enrolledByEmail == null)
        {
            return Unauthorized("Token without email is invalid.");
        }

        var enrollmentDto = await this.enrollmentService.CreateEnrollmentByClassAsync(request, enrolledByEmail, cancellationToken);

        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = enrollmentDto.EnrollmentId },
            enrollmentDto);
    }

    [HttpPost("course")]
    public async Task<ActionResult<EnrollmentCourseDto>> CreateEnrollmentByCourseAsync(CreateEnrollmentByCourseRequest request, CancellationToken cancellationToken)
    {
        var enrolledByEmail = GetUserEmail();

        if (enrolledByEmail == null)
        {
            return Unauthorized("Token without email is invalid.");
        }

        var enrollmentDto = await this.enrollmentService.CreateEnrollmentByCourseAsync(request, enrolledByEmail, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, enrollmentDto);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<EnrollmentDto>> UpdateEnrollmentAsync(Guid id, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.UpdateEnrollmentAsync(id, updateEnrollmentRequest, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteEnrollmentAsync(DeleteEnrollmentRequest deleteEnrollmentRequest, CancellationToken cancellationToken)
    {
        await this.enrollmentService.DeleteEnrollmentAsync(deleteEnrollmentRequest, cancellationToken);
        return NoContent();
    }

    private string? GetUserEmail()
    {
        var email = this.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        return email;
    }
}
