using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Extensions;
using CourseManagement.Business.Mappers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<StudentService> logger;

    public StudentService(ApplicationDbContext dbContext, ILogger<StudentService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<PageResult<StudentDto>> GetStudentsAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .AsNoTracking()
                         .GetItems(@params,
                                   s => StudentMapper.MapsToStudentDto(s), 
                                   s => s.StudentId, 
                                   cancellationToken);
    }

    public async Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await this.dbContext
                                .Students
                                .SingleOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(studentId);
        }

        return StudentMapper.MapsToStudentDto(student);
    }

    public async Task<StudentDto> GetStudentByRollNoAsync(string studentRollNumber, CancellationToken cancellationToken)
    {
        var student = await this.dbContext
                                .Students
                                .SingleOrDefaultAsync(s => s.RollNumber == studentRollNumber, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(studentRollNumber);
        }

        return StudentMapper.MapsToStudentDto(student);
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken)
    {
        if (await IsStudentNumberExists(createStudentRequest.RollNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Student with roll number {createStudentRequest.RollNumber} already exists.");
        }

        var user = await this.dbContext
                             .Users
                             .Where(u => u.EmailAddress == createStudentRequest.EmailAddress)
                             .Where(u => u.UserUserRoles.Any(uur => uur.UserRole.RoleName == nameof(UserRoles.Student)))
                             .Select(u => new
                             {
                                 u.UserId,
                                 u.EmailAddress
                             })
                             .SingleOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(createStudentRequest.EmailAddress);
        }

        var student = StudentMapper.MapsToStudent(createStudentRequest, user.UserId);

        await this.dbContext.AddAsync(student, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New student with roll number {Number} has been created.", createStudentRequest.RollNumber);

        return StudentMapper.MapsToStudentDto(student);
    }

    public async Task<StudentDto> UpdateStudentByIdAsync(Guid studentId, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken)
    {
        var student = await this.dbContext
                                .Students
                                .SingleOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(studentId);
        }

        if (!string.IsNullOrEmpty(updateStudentRequest.RollNumber) && await IsStudentNumberExists(updateStudentRequest.RollNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Student with roll number {updateStudentRequest.RollNumber} already exists.");
        }

        StudentMapper.UpdateStudent(student, updateStudentRequest);

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Student with studentId {StudentId} and userId {UserId} is updated.", student.StudentId, student.UserId);

        return StudentMapper.MapsToStudentDto(student);
    }

    public async Task DeleteStudentAsync(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                  .Students
                  .Where(s => s.StudentId == deleteStudentRequest.StudentId)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Student with roll number {Num} is deleted.", deleteStudentRequest.StudentId);
    }

    private async Task<bool> IsStudentNumberExists(string studentNumber, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .AnyAsync(s => s.RollNumber == studentNumber, cancellationToken);
    }
}
