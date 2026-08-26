using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
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
    public async Task<ActionResult<CourseDto>> GetCourses([AsParameters] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCoursesAsync(@params, cancellationToken));
    }

    [HttpGet("{courseId}")]
    public async Task<ActionResult<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCourseByIdAsync(courseId, cancellationToken));
    }

    [HttpGet("course-name/{courseName}")]
    public async Task<ActionResult<CourseDto>> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCourseByNameAsync(courseName, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken)
    {
        return CreatedAtAction(
                nameof(CreateCourse), 
                await this.courseService.CreateCourseAsync(createCourseRequest, cancellationToken));
    }

    [HttpPatch("{courseId}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.UpdateCourseByIdAsync(courseId, updateCourseRequest, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCourse(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken)
    {
        await this.courseService.DeleteCourseByIdAsync(deleteCourseRequest, cancellationToken);
        return NoContent();
    }
}