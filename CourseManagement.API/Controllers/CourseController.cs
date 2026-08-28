using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
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
public class CourseController : ControllerBase
{
    private readonly ICourseService courseService;

    public CourseController(ICourseService courseService)
    {
        this.courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<CourseDto>>> GetCourses([FromQuery] PaginationParams @params, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCoursesAsync(@params, cancellationToken));
    }

    [HttpGet("{courseId}")]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid courseId, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCourseByIdAsync(courseId, cancellationToken));
    }

    [HttpGet("course-name/{courseName}")]
    public async Task<ActionResult<CourseDto>> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetCourseByNameAsync(courseName, cancellationToken));
    }

    [HttpGet("{courseId}/students")]
    public async Task<ActionResult<PageResult<StudentDto>>> GetStudentsByCourse([FromQuery] PaginationParams @params, [FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetStudentsByCourseAsync(@params, courseId, cancellationToken));
    }

    [HttpGet("{courseId}/classes")]
    public async Task<ActionResult<PageResult<ClassDto>>> GetClassesByCourse([FromQuery] PaginationParams @params, [FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        return Ok(await this.courseService.GetClassesByCourseAsync(@params, courseId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken)
    {
        var courseDto = await this.courseService.CreateCourseAsync(createCourseRequest, cancellationToken);
        return CreatedAtAction(
                nameof(GetCourseById), 
                new { courseId = courseDto.CourseId },
                courseDto);
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