using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = nameof(UserPolicies.AdminOrStaff))]
public class ClassController : ControllerBase
{
    private readonly IClassService classService;

    public ClassController(IClassService classService)
    {
        this.classService = classService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<ClassDto>>> GetAllClasses([FromQuery] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassesAsync(@params, cancellationToken));
    }

    [HttpGet("instructor-email/{email}")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByInstructorEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassesByInstructorEmail(email, cancellationToken));
    }

    [HttpGet("{classId}")]
    public async Task<ActionResult<ClassDto>> GetClassById(Guid classId, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassByIdAsync(classId, cancellationToken));
    }

    [HttpGet("class-name/{className}")]
    public async Task<ActionResult<ClassDto>> GetClassByNameAsync(string className, CancellationToken cancellationToken)
    {
        return Ok(await this.classService.GetClassByNameAsync(className, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClassDto>> CreateClass(CreateClassRequest createClassRequest, CancellationToken cancellationToken)
    {
        var classDto = await this.classService.CreateClassAsync(createClassRequest, cancellationToken);
        return CreatedAtAction(
            nameof(GetClassById),
            new { classId = classDto.ClassId },
            classDto);
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
