using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.Staff)}")]
public class CourseController : ControllerBase
{
    private readonly ICourseService courseService;

    public CourseController(ICourseService courseService)
    {
        this.courseService = courseService;
    }

    [HttpGet]
    public IActionResult GetCourses()
    {
        throw new NotImplementedException(); // TODO: Pagination
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<CourseDto>> GetCourseByNameAsync(string name, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCourseByNameAsync(name, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken)
    {
        return CreatedAtAction(
                nameof(CreateCourse), 
                await this.courseService.CreateCourseAsync(createCourseRequest, cancellationToken));
    }

    [HttpPatch("course-name/{name}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(string name, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.UpdateCourseByNameAsync(name, updateCourseRequest, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCourse(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken)
    {
        await this.courseService.DeleteCourseByNameAsync(deleteCourseRequest, cancellationToken);
        return NoContent();
    }
}