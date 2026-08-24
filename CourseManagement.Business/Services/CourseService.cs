using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.CourseDTOs;
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

    public async Task<CourseDto> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
    {
        var course = await this.dbContext
                         .Courses.AsNoTracking()
                         .Include(c => c.CourseClasses)
                         .ThenInclude(cc => cc.Class)
                         .FirstOrDefaultAsync(c => c.Name == courseName, cancellationToken);

        if (course == null)
        {
            throw new CourseNotFoundException(courseName);
        }

        var classNames = course.CourseClasses.Select(cc => cc.Class.Name);

        return CourseMapper.MapsToCourseDto(course, classNames);
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

    public async Task<CourseDto> UpdateCourseByNameAsync(string courseName, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken)
    {
        var course = await this.dbContext
                               .Courses
                               .Include(c => c.CourseClasses)
                               .ThenInclude(cc => cc.Class)
                               .FirstOrDefaultAsync(c => c.Name == courseName, cancellationToken);

        if (course == null)
        {
            throw new CourseNotFoundException(courseName);
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

    public async Task DeleteCourseByNameAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                  .Courses
                  .Where(c => c.Name == deleteCourseRequest.Name)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Course named {deleteCourseRequest} has been deleted.", deleteCourseRequest.Name);
    }

    private async Task<bool> IsCourseNameExists(string courseName, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Courses
                         .AnyAsync(c => c.Name == courseName, cancellationToken);
    }

    private async Task<List<ClassInfo>> GetClassesAsync(IEnumerable<string> classNames, CancellationToken cancellationToken)
    {
        var classes = await this.dbContext
                                .Classes.AsNoTracking()
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
