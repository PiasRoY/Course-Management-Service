using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Extensions;
using CourseManagement.Business.Mappers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class ClassService : IClassService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ClassService> logger;

    public ClassService(ApplicationDbContext dbContext, ILogger<ClassService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<PageResult<ClassDto>> GetClassesAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Classes
                         .AsNoTracking()
                         .OrderBy(cl => cl.CreatedAt)
                         .ThenBy(cl => cl.ClassId)
                         .Select(ClassMapper.ProjectToClasDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<ClassDto> GetClassByNameAsync(string className, CancellationToken cancellationToken)
    {
        var classDto = await this.dbContext
                                 .Classes
                                 .AsNoTracking()
                                 .Select(ClassMapper.ProjectToClasDto)
                                 .SingleOrDefaultAsync(cl => cl.Name.Equals(className, StringComparison.), cancellationToken);

        return classDto ?? throw new ClassNotFoundException(className);
    }

    public async Task<ClassDto> GetClassByIdAsync(Guid classId, CancellationToken cancellationToken)
    {
        var classDto = await this.dbContext
                               .Classes
                               .AsNoTracking()
                               .Select(ClassMapper.ProjectToClasDto)
                               .SingleOrDefaultAsync(cl => cl.ClassId == classId, cancellationToken);

        return classDto ?? throw new ClassNotFoundException(classId);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByInstructorEmailAsync(string email, CancellationToken cancellationToken)
    {
        var isExists = await this.dbContext
                                 .Users
                                 .AsNoTracking()
                                 .AnyAsync(u => u.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (!isExists)
        {
            throw new InstructorNotFoundException(email);
        }

        return await this.dbContext
                         .Classes
                         .AsNoTracking()
                         .Where(cl => cl.Instructor.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase))
                         .Select(ClassMapper.ProjectToClasDto)
                         .ToListAsync(cancellationToken);
    }

    public async Task<PageResult<StudentDto>> GetStudentsByClassId(PaginationParams @params, Guid classId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .AsNoTracking()
                         .Where(s => s.Enrollments.Any(e => e.ClassId == classId))
                         .OrderBy(s => s.CreatedAt)
                         .ThenBy(s => s.StudentId)
                         .Select(StudentMapper.ProjectToStudentDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<PageResult<CourseDto>> GetCoursesByClassIdAsync(PaginationParams @params, Guid classId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Courses
                         .AsNoTracking()
                         .Where(c => c.CourseClasses.Any(cc => cc.ClassId == classId))
                         .OrderBy(c => c.CreatedAt)
                         .ThenBy(c => c.CourseId)
                         .Select(CourseMapper.ProjectToCourseDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken)
    {
        var isClassExists = await this.dbContext
                                      .Classes
                                      .AnyAsync(cl => cl.Name.Equals(createClassRequest.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (isClassExists)
        {
            throw new InvalidOperationException("Class name already exists.");
        }

        var instructor = await this.GetInstructor(createClassRequest.InstructorEmail, cancellationToken);
        var @class = ClassMapper.MapsToClass(createClassRequest, instructor);

        await this.dbContext.Classes.AddAsync(@class, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} is created.", @class.Name);

        return ClassMapper.MapsToClassDto(@class, instructor);
    }

    public async Task<ClassDto> UpdateClassByIdAsync(Guid classId, UpdateClassRequest updateClassRequest, CancellationToken cancellationToken)
    {
        var @class = await this.dbContext
                               .Classes
                               .Include(cl => cl.Instructor)
                               .SingleOrDefaultAsync(cl => cl.ClassId == classId, cancellationToken);

        if (@class == null)
        {
            throw new ClassNotFoundException(classId);
        }

        ClassMapper.MapsStaticPropertiesToClass(updateClassRequest, @class);

        User instructor = null!;
        if (updateClassRequest.InstructorEmail != null)
        {
            instructor = await this.GetInstructor(updateClassRequest.InstructorEmail, cancellationToken);
            @class.InstructorId = instructor.UserId;
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} has been updated.", @class.Name);

        return ClassMapper.MapsToClassDto(@class, updateClassRequest.InstructorEmail != null ? instructor : @class.Instructor);
    }

    public async Task DeleteClassByIdAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                  .Classes
                  .Where(cl => cl.ClassId == deleteClassRequest.ClassId)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} has been deleted.", deleteClassRequest.ClassId);
    }

    private async Task<User> GetInstructor(string InstructorEmail, CancellationToken cancellationToken)
    {
        var instructor = await this.dbContext
                                   .Users
                                   .AsNoTracking()
                                   .Where(u => u.EmailAddress.Equals(InstructorEmail, StringComparison.OrdinalIgnoreCase))
                                   .Where(u => u.UserUserRoles.Any(uur => uur.UserRole.RoleName == UserRoles.Instructor.ToString()))
                                   .SingleOrDefaultAsync(cancellationToken);

        if (instructor == null)
        {
            throw new InstructorNotFoundException(InstructorEmail);
        }

        return instructor;
    }
}

