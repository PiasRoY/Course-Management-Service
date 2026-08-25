using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class ClassController : ControllerBase
{
    private readonly IClassService classService;

    public ClassController(IClassService classService)
    {
        this.classService = classService;
    }

    [HttpGet]
    public IActionResult GetAllClasses()
    {
        throw new NotImplementedException(); // TODO: Pagination.
    }

    [HttpGet("instructor-email/{email}")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByInstructorEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassesByInstructorEmail(email, cancellationToken));
    }

    [HttpGet("{classId}")]
    public async Task<ActionResult<ClassDto>> GetClassByIdAsync(Guid classId, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassByIdAsync(classId, cancellationToken));
    }

    [HttpGet("class-name/{className}")]
    public async Task<ActionResult<ClassDto>> GetClassByIdAsync(string className, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassByNameAsync(className, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClassDto>> CreateClass(CreateClassRequest createClassRequest, CancellationToken cancellationToken)
    {
        return CreatedAtAction(
            nameof(CreateClass),
            await this.classService.CreateClassAsync(createClassRequest, cancellationToken));
    }

    [HttpPatch("{classId}")]
    public async Task<ActionResult<ClassDto>> UpdateClassAsync(Guid classId, UpdateClassRequest updateClassRequest, CancellationToken cancellationToken)
    {
        var classDto = await this.classService.UpdateClassByIdAsync(classId, updateClassRequest, cancellationToken);
        return Ok(classDto);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteClassAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken)
    {
        await this.classService.DeleteClassByIdAsync(deleteClassRequest, cancellationToken);
        return NoContent();
    }
}
