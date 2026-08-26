using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
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
                         .GetItems(@params,
                                   cl => ClassMapper.MapsToClassDto(cl),
                                   cl => cl.ClassId,
                                   cancellationToken);
    }

    public async Task<ClassDto> GetClassByNameAsync(string className, CancellationToken cancellationToken)
    {
        var @class = await this.dbContext
                               .Classes.AsNoTracking()
                               .Include(cl => cl.Instructor)
                               .SingleOrDefaultAsync(cl => cl.Name == className, cancellationToken);

        if (@class == null)
        {
            throw new ClassNotFoundException(className);
        }

        return ClassMapper.MapsToClassDto(@class);
    }

    public async Task<ClassDto> GetClassByIdAsync(Guid classId, CancellationToken cancellationToken)
    {
        var @class = await this.dbContext
                               .Classes.AsNoTracking()
                               .Include(cl => cl.Instructor)
                               .SingleOrDefaultAsync(cl => cl.ClassId == classId, cancellationToken);

        if (@class == null)
        {
            throw new ClassNotFoundException(classId);
        }

        return ClassMapper.MapsToClassDto(@class);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByInstructorEmail(string email, CancellationToken cancellationToken)
    {
        var instructor = await this.GetInstructor(email, cancellationToken);

        return await this.dbContext
                         .Classes
                         .Where(cl => cl.Instructor.EmailAddress == email)
                         .Select(cl => ClassMapper.MapsToClassDto(cl, instructor))
                         .ToListAsync(cancellationToken);
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken)
    {
        var isClassExists = await this.dbContext
                                      .Classes
                                      .AnyAsync(cl => cl.Name == createClassRequest.Name, cancellationToken);

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
                                   .Where(u => u.EmailAddress == InstructorEmail)
                                   .Where(u => u.UserUserRoles.Any(uur => uur.UserRole.RoleName == UserRoles.Instructor.ToString()))
                                   .SingleOrDefaultAsync(cancellationToken);

        if (instructor == null)
        {
            throw new InstructorNotFoundException(InstructorEmail);
        }

        return instructor;
    }
}

