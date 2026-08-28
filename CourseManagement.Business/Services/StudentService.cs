using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
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
                         .OrderBy(s => s.CreatedAt)
                         .ThenBy(s => s.StudentId)
                         .Select(StudentMapper.ProjectToStudentDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var studentDto = await this.dbContext
                                .Students
                                .Select(StudentMapper.ProjectToStudentDto)
                                .SingleOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        return studentDto ?? throw new StudentNotFoundException(studentId);
    }

    public async Task<StudentDto> GetStudentByRollNoAsync(string studentRollNumber, CancellationToken cancellationToken)
    {
        var studentDto = await this.dbContext
                                .Students
                                .Where(s => s.RollNumber == studentRollNumber)
                                .Select(StudentMapper.ProjectToStudentDto)
                                .SingleOrDefaultAsync(cancellationToken);

        return studentDto ?? throw new StudentNotFoundException(studentRollNumber);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByStudent(Guid userId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Classes
                         .Where(cl => cl.Enrollments.Any(e => e.Student.UserId == userId))
                         .Select(ClassMapper.ProjectToClasDto)
                         .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesByStudent(Guid userId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Courses
                         .Where(c => c.Enrollments.Any(e => e.Student.UserId == userId))
                         .Select(CourseMapper.ProjectToCourseDto)
                         .ToListAsync(cancellationToken);
    }

    public async Task<PageResult<StudentDto>> GetClassMatesByStudent(Guid userId, PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .Where(s => s.UserId != userId)
                         .Where(s => s.Enrollments.Any(e => e.Class.Enrollments.Any(cle => cle.Student.UserId == userId)))
                         .OrderBy(s => s.CreatedAt)
                         .ThenBy(s => s.StudentId)
                         .Select(StudentMapper.ProjectToStudentDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<PageResult<StudentDto>> GetCourseMatesByStudent(Guid userId, PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Students
                         .Where(s => s.UserId != userId)
                         .Where(s => s.Enrollments.Any(e => e.Course != null &&  e.Course.Enrollments.Any(ce => ce.Student.UserId == userId)))
                         .OrderBy(s => s.CreatedAt)
                         .ThenBy(s => s.StudentId)
                         .Select(StudentMapper.ProjectToStudentDto)
                         .GetItemsAsync(@params, cancellationToken);
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
                             .SingleOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(createStudentRequest.EmailAddress);
        }

        var student = StudentMapper.MapsToStudent(createStudentRequest, user.UserId);

        await this.dbContext.AddAsync(student, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New student with roll number {Number} has been created.", createStudentRequest.RollNumber);

        return StudentMapper.MapsToStudentDto(student, user);
    }

    public async Task<StudentDto> UpdateStudentByIdAsync(Guid studentId, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken)
    {
        var student = await this.dbContext
                                .Students
                                .Include(s => s.User)
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

        return StudentMapper.MapsToStudentDto(student, student.User);
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
