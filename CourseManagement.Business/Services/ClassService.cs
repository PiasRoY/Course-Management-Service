using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.ClassDTOs;
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

    public async Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken)
    {
        var isClassExists = await this.dbContext
                                        .Classes.AsNoTracking()
                                        .AnyAsync(cl => cl.Name == createClassRequest.Name, cancellationToken);

        if (isClassExists)
        {
            throw new InvalidOperationException("Class name already exists.");
        }

        var instructor = await this.GetInstructor(createClassRequest.InstructorEmail, cancellationToken);

        var @class = new Class
        {
            ClassId = Guid.NewGuid(),
            Name = createClassRequest.Name,
            Semester = createClassRequest.Semester,
            Year = createClassRequest.Year,
            SectionCode = createClassRequest.SectionCode,
            Instructor = instructor
        };

        await this.dbContext.Classes.AddAsync(@class, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} is created.", @class.Name);

        return ClassMapper.MapsToClassDto(@class);
    }

    public async Task<ClassDto> UpdateClassByNameAsync(UpdateClassRequest updateClassRequest, CancellationToken cancellationToken)
    {
        var @class = await this.dbContext
                                .Classes
                                .FirstOrDefaultAsync(cl => cl.Name == updateClassRequest.Name, cancellationToken);

        if (@class == null)
        {
            throw new ClassNotFoundException(updateClassRequest.Name);
        }

        ClassMapper.MapsStaticPropertiesToClass(updateClassRequest, @class);

        if (updateClassRequest.InstructorEmail != null)
        {
            var instructor = await this.GetInstructor(updateClassRequest.InstructorEmail, cancellationToken);
            @class.InstructorId = instructor.UserId;
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} has been updated.", @class.Name);

        return ClassMapper.MapsToClassDto(@class);
    }

    public async Task DeleteClassByNameAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                    .Classes
                    .Where(cl => cl.Name == deleteClassRequest.Name)
                    .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Class named {Name} has been deleted.", deleteClassRequest.Name);
    }

    private async Task<User> GetInstructor(string InstructorEmail, CancellationToken cancellationToken)
    {
        var instructor = await this.dbContext
                                    .Users.AsNoTracking()
                                    .Where(u => u.EmailAddress == InstructorEmail)
                                    .FirstOrDefaultAsync(cancellationToken);

        if (instructor == null)
        {
            throw new InstructorNotFoundException(InstructorEmail);
        }

        return instructor;
    }
}
