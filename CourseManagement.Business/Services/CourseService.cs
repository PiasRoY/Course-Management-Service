using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Extensions;
using CourseManagement.Business.Mappers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<CourseService> logger;

    public CourseService(ApplicationDbContext dbContext, ILogger<CourseService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<PageResult<CourseDto>> GetCoursesAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Courses
                         .OrderBy(c => c.CreatedAt)
                         .ThenBy(c => c.CourseId)
                         .Select(CourseMapper.ProjectToCourseDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var courseDto = await this.dbContext
                         .Courses
                         .Select(CourseMapper.ProjectToCourseDto)
                         .SingleOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);

        return courseDto ?? throw new CourseNotFoundException(courseId);
    }

    public async Task<CourseDto> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
    {
        var courseDto = await this.dbContext
                         .Courses
                         .Select(CourseMapper.ProjectToCourseDto)
                         .SingleOrDefaultAsync(c => c.Name.Equals(courseName, StringComparison.OrdinalIgnoreCase), cancellationToken);

        return courseDto ?? throw new CourseNotFoundException(courseName);
    }

    public async Task<PageResult<StudentDto>> GetStudentsByCourseAsync(PaginationParams @params, Guid courseId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
                         .OrderBy(s => s.CreatedAt)
                         .ThenBy(s => s.StudentId)
                         .Select(StudentMapper.ProjectToStudentDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<PageResult<ClassDto>> GetClassesByCourseAsync(PaginationParams @params, Guid courseId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Classes
                         .Where(cl => cl.CourseClasses.Any(cc => cc.CourseId == courseId))
                         .OrderBy(cl => cl.CreatedAt)
                         .ThenBy(cl => cl.ClassId)
                         .Select(ClassMapper.ProjectToClasDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken)
    {
        if (await IsCourseNameExists(createCourseRequest.Name, cancellationToken))
        {
            throw new InvalidOperationException("Course name already exists.");
        }

        var classInfos = await this.GetClassesAsync(createCourseRequest.ClassNames, cancellationToken);
        var course = CourseMapper.MapsToCourse(createCourseRequest, classInfos.Select(cl => cl.Id));

        await this.dbContext.AddAsync(course, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New course named {Name} is created.", course.Name);

        return CourseMapper.MapsToCourseDto(course, classInfos.Select(cl => cl.Name));
    }

    public async Task<CourseDto> UpdateCourseByIdAsync(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken)
    {
        var course = await this.dbContext
                               .Courses
                               .Include(c => c.CourseClasses)
                               .ThenInclude(cc => cc.Class)
                               .SingleOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);

        if (course == null)
        {
            throw new CourseNotFoundException(courseId);
        }

        if (!string.IsNullOrEmpty(updateCourseRequest.Name) && await IsCourseNameExists(updateCourseRequest.Name, cancellationToken))
        {
            throw new InvalidOperationException("Course name already exists.");
        }

        List<ClassInfo> classInfos = [];
        
        if (updateCourseRequest.ClassNames != null)
        {
            classInfos = await this.GetClassesAsync(updateCourseRequest.ClassNames, cancellationToken);
            course.CourseClasses.Clear();
        }

        CourseMapper.UpdateCourseFromCourseDto(course, updateCourseRequest, classInfos.Select(cl => cl.Id));

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Course named {Name} has been updated.", course.Name);

        return CourseMapper.MapsToCourseDto(course, classInfos.Select(cl => cl.Name));
    }

    public async Task DeleteCourseByIdAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                  .Courses
                  .Where(c => c.CourseId == deleteCourseRequest.CourseId)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Course named {deleteCourseRequest} has been deleted.", deleteCourseRequest.CourseId);
    }

    private async Task<bool> IsCourseNameExists(string courseName, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Courses
                         .AnyAsync(c => c.Name.Equals(courseName, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    private async Task<List<ClassInfo>> GetClassesAsync(IEnumerable<string> classNames, CancellationToken cancellationToken)
    {
        var classes = await this.dbContext
                                .Classes
                                .Where(cl => classNames.Contains(cl.Name))
                                .Select(cl => new ClassInfo
                                (
                                    cl.ClassId,
                                    cl.Name
                                ))
                                .ToListAsync(cancellationToken);

        if (classes.Count != classNames.Count())
        {
            var notFoundClasses = classNames.Except(classes.Select(cl => cl.Name));
            throw new ClassNotFoundException(notFoundClasses);
        }

        return classes;
    }

    private record ClassInfo(Guid Id, string Name);
}
