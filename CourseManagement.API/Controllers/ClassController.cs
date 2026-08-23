using CourseManagement.Business.Constants;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = UserRoles.AdminOrStaff)]
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

    [HttpGet("class-name/{className}")]
    public async Task<ActionResult<ClassDto>> GetClassByNameAsync(string className, CancellationToken cancellationToken)
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

    [HttpPatch]
    public async Task<ActionResult<ClassDto>> UpdateClassAsync(UpdateClassRequest updateClassRequest, CancellationToken cancellationToken)
    {
        var classDto = await this.classService.UpdateClassByNameAsync(updateClassRequest, cancellationToken);
        return Ok(classDto);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteClassAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken)
    {
        await this.classService.DeleteClassByNameAsync(deleteClassRequest, cancellationToken);
        return NoContent();
    }
}
