using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.Services;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourseManagement.UnitTests.Services.Tests;

public class ClassServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly ClassService classService;
    private readonly User instructor;
    private readonly Class classEntity;

    public ClassServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());

        var instructorRole = new UserRole
        {
            RoleId = Guid.NewGuid(),
            RoleName = UserRoles.Instructor.ToString()
        };

        this.dbContext.UserRoles.Add(instructorRole);

        this.instructor = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "instructor@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Instructor",
            UserUserRoles = [new UserUserRole { UserRole = instructorRole }]
        };
        
        this.dbContext.Users.Add(this.instructor);

        this.classEntity = new Class
        {
            ClassId = Guid.NewGuid(),
            Name = "Physics",
            Semester = Semester.Fall,
            Year = 2026,
            SectionCode = "A",
            InstructorId = this.instructor.UserId
        };
        
        this.dbContext.Classes.Add(this.classEntity);
        
        this.dbContext.SaveChanges();
        
        this.classService = new ClassService(this.dbContext, Mock.Of<ILogger<ClassService>>());
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task CreateClassAsync_WithValidRequest_CreatesClassAndReturnsClassDto()
    {
        var result = await this.classService.CreateClassAsync(new CreateClassRequest
        {
            Name = "Chemistry",
            Semester = Semester.Fall,
            Year = 2026,
            SectionCode = "A",
            InstructorEmail = this.instructor.EmailAddress
        }, CancellationToken.None);

        Assert.Equal("Chemistry", result.Name);
        Assert.Equal("Pias Instructor", result.InstructorName);
        Assert.Equal(2, await this.dbContext.Classes.CountAsync());
    }

    [Fact]
    public async Task UpdateClassByIdAsync_WithValidRequest_UpdatesClassAndReturnsClassDto()
    {
        var result = await this.classService.UpdateClassByIdAsync(this.classEntity.ClassId, new UpdateClassRequest
        {
            ClassName = "Advanced Physics",
            SectionCode = "B"
        }, CancellationToken.None);

        Assert.Equal("Advanced Physics", result.Name);
        Assert.Equal("B", result.SectionCode);
    }

    [Fact]
    public async Task GetClassByIdAsync_WithExistingClass_ReturnsClassDto()
    {
        var result = await this.classService.GetClassByIdAsync(this.classEntity.ClassId, CancellationToken.None);

        Assert.Equal(this.classEntity.ClassId, result.ClassId);
        Assert.Equal("Physics", result.Name);
        Assert.Equal("instructor@example.com", result.InstructorEmail);
    }
}
