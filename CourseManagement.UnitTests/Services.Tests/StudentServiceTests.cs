using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.Services;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourseManagement.UnitTests.Services.Tests;

public class StudentServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly StudentService studentService;
    private readonly User user;
    private readonly Student student;
    private readonly UserRole studentRole; 

    public StudentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());

        this.studentRole = new UserRole
        {
            RoleId = Guid.NewGuid(),
            RoleName = UserRoles.Student.ToString()
        };

        this.user = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = [new UserUserRole { UserRole = studentRole }]
        };

        this.student = new Student
        {
            StudentId = Guid.NewGuid(),
            UserId = this.user.UserId,
            RollNumber = "ST-001",
            Status = StudentStatus.Active,
            AdmissionDate = DateTime.UtcNow,
            CurrentTerm = 1,
            CurrentSemester = 1,
            User = this.user
        };

        this.dbContext.UserRoles.Add(studentRole);
        this.dbContext.Students.Add(this.student);
        this.dbContext.SaveChanges();

        this.studentService = new StudentService(this.dbContext, Mock.Of<ILogger<StudentService>>());
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task CreateStudentAsync_WithValidRequest_CreatesStudentAndReturnsStudentDto()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student2@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = [new UserUserRole { UserRole = studentRole }]
        };

        await this.dbContext.AddAsync(user);
        await this.dbContext.SaveChangesAsync();

        var result = await this.studentService.CreateStudentAsync(new CreateStudentRequest
        {
            EmailAddress = "student2@example.com",
            RollNumber = "ST-002",
            Status = StudentStatus.Active,
            AdmissionDate = DateTime.UtcNow,
            CurrentTerm = 1,
            CurrentSemester = 1
        }, CancellationToken.None);

        Assert.Equal("ST-002", result.StudentNumber);
        Assert.Equal("student2@example.com", result.Email);
        Assert.Equal(2, await this.dbContext.Students.CountAsync());
    }

    [Fact]
    public async Task UpdateStudentByIdAsync_WithValidRequest_UpdatesStudentAndReturnsStudentDto()
    {
        var result = await this.studentService.UpdateStudentByIdAsync(this.student.StudentId, new UpdateStudentRequest
        {
            RollNumber = "ST-002",
            Status = StudentStatus.Suspended,
            CurrentTerm = 2
        }, CancellationToken.None);

        Assert.Equal("ST-002", result.StudentNumber);
        Assert.Equal(StudentStatus.Suspended, result.Status);
        Assert.Equal(2, result.CurrentTerm);
    }

    [Fact]
    public async Task GetStudentByIdAsync_WithExistingStudent_ReturnsStudentDto()
    {
        var result = await this.studentService.GetStudentByIdAsync(this.student.StudentId, CancellationToken.None);

        Assert.Equal(this.student.StudentId, result.StudentId);
        Assert.Equal("student@example.com", result.Email);
        Assert.Equal("Pias Student", result.FullName);
        Assert.Equal("ST-001", result.StudentNumber);
    }
}
