using CourseManagement.Business.DTOs.EnrollmentDTOs;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.GetEnrollmentByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollmentByClassAsync(CreateEnrollmentByClassRequest request, CancellationToken cancellationToken)
    {
        var enrollmentDto = await this.enrollmentService.CreateEnrollmentByClassAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = enrollmentDto.EnrollmentId },
            enrollmentDto);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollmentByCourseAsync(CreateEnrollmentByCourseRequest request, CancellationToken cancellationToken)
    {
        var enrollmentDto = await this.enrollmentService.CreateEnrollmentByCourseAsync(request, cancellationToken);
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
}
