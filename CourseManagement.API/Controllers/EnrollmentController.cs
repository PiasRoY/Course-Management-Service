using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        this.enrollmentService = enrollmentService;
    }

    [HttpGet]
    public IActionResult GetEnrollments()
    {
        throw new NotImplementedException(); // Pagination.
    }

    [HttpGet("{id}")]
    public IActionResult GetEnrollmentById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(this.enrollmentService.GetEnrollmentByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollment(CreateEnrollmentRequest createEnrollmentRequest, CancellationToken cancellationToken)
    {
        return CreatedAtAction(nameof(CreateEnrollment),
                               await this.enrollmentService.CreateEnrollmentAsync(createEnrollmentRequest, cancellationToken));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateEnrollment(Guid id, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.enrollmentService.UpdateEnrollmentAsync(id, updateEnrollmentRequest, cancellationToken));
    }

    [HttpDelete]
    public IActionResult DeleteEnrollment(DeleteEnrollmentRequest deleteEnrollmentRequest, CancellationToken cancellationToken)
    {
        this.enrollmentService.DeleteEnrollmentAsync(deleteEnrollmentRequest, cancellationToken);
        return NoContent();
    }
}
