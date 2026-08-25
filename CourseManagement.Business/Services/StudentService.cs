using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Enums;
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

    public async Task<StudentDto> CreateStudentByRollNoAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken)
    {
        if (await IsStudentNumberExists(createStudentRequest.RollNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Student with roll number {createStudentRequest.RollNumber} already exists.");
        }

        var user = await this.dbContext
                             .Users
                             .Select(u => new
                             {
                                 u.UserId,
                                 u.EmailAddress,
                                 Role = u.UserUserRoles.Select(uur => uur.UserRole.RoleName)
                             })
                             .SingleOrDefaultAsync(u => u.EmailAddress == createStudentRequest.EmailAddress, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(createStudentRequest.EmailAddress);
        }

        if (!user.Role.ToList().Contains(UserRoles.Student.ToString()))
        {
            throw new InvalidOperationException("User does not have student role access.");
        }

        var student = StudentMapper.MapsToStudent(createStudentRequest, user.UserId);

        await this.dbContext.AddAsync(user, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New student with roll number {Number} has been created.", createStudentRequest.RollNumber);

        return StudentMapper.MapsToStudentDto(student);
    }

    public async Task<StudentDto> UpdateStudentByRollNoAsync(string studentNumber, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken)
    {
        var student = await this.dbContext
                                .Students
                                .SingleOrDefaultAsync(s => s.RollNumber == studentNumber, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(studentNumber);
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
                  .Where(s => s.RollNumber == deleteStudentRequest.RollNumber)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Student with roll number {Num} is deleted.", deleteStudentRequest.RollNumber);
    }

    private async Task<bool> IsStudentNumberExists(string studentNumber, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .AnyAsync(s => s.RollNumber == studentNumber, cancellationToken);
    }
}
