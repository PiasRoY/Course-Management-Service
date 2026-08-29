using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.Services;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourseManagement.UnitTests.Services.Tests;

public class EnrollmentServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly EnrollmentService enrollmentService;
    private readonly Student studentEntity;
    private readonly Class classEntity;
    private readonly Course courseEntity;
    private readonly string enrolledBy;

    public EnrollmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());

        this.enrolledBy = "pias@tester.com";

        var user = new User
        {
            UserId = Guid.NewGuid(),
            EmailAddress = "student@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Pias",
            LastName = "Student",
            UserUserRoles = []
        };
        this.studentEntity = new Student
        {
            StudentId = Guid.NewGuid(),
            UserId = user.UserId,
            RollNumber = "ST-001",
            Status = StudentStatus.Active,
            AdmissionDate = DateTime.UtcNow,
            CurrentTerm = 1,
            CurrentSemester = 1,
            User = user
        };
        this.classEntity = new Class
        {
            ClassId = Guid.NewGuid(),
            Name = "Physics",
            Semester = Semester.Fall,
            Year = 2026,
            SectionCode = "A"
        };
        this.courseEntity = new Course
        {
            CourseId = Guid.NewGuid(),
            Name = "Science",
            Title = "Introduction to Science",
            Credits = 3,
            CourseClasses = [new CourseClass { Class = this.classEntity, ClassId = this.classEntity.ClassId }]
        };

        this.dbContext.Students.Add(this.studentEntity);
        this.dbContext.Courses.Add(this.courseEntity);
        this.dbContext.SaveChanges();

        this.enrollmentService = new EnrollmentService(this.dbContext, Mock.Of<ILogger<EnrollmentService>>());
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task CreateEnrollmentByClassAsync_WithValidRequest_CreatesEnrollment()
    {
        var result = await this.enrollmentService.CreateEnrollmentByClassAsync(new CreateEnrollmentByClassRequest
        {
            StudentId = this.studentEntity.StudentId,
            ClassId = this.classEntity.ClassId
        }, enrolledBy, CancellationToken.None);

        Assert.Equal(this.studentEntity.StudentId, result.StudentId);
        Assert.Equal(this.classEntity.ClassId, result.ClassId);
        Assert.Equal(enrolledBy, result.EnrolledBy);
        Assert.Single(await this.dbContext.Enrollments.ToListAsync());
    }

    [Fact]
    public async Task CreateEnrollmentByCourseAsync_WithValidRequest_CreatesEnrollment()
    {
        var result = await this.enrollmentService.CreateEnrollmentByCourseAsync(new CreateEnrollmentByCourseRequest
        {
            StudentId = this.studentEntity.StudentId,
            CourseId = this.courseEntity.CourseId
        }, enrolledBy, CancellationToken.None);

        Assert.Equal(this.studentEntity.StudentId, result.StudentId);
        Assert.Equal(this.courseEntity.CourseId, result.CourseId);
        Assert.Equal(enrolledBy, result.EnrolledBy);
        Assert.Single(await this.dbContext.Enrollments.ToListAsync());
    }

    [Fact]
    public async Task CreateEnrollmentByClassNamesAsync_WithValidRequest_CreatesEnrollment()
    {
        var result = await this.enrollmentService.CreateEnrollmentByClassNamesAsync(new CreateEnrollmentByClassNames
        {
            StudentRollNumber = this.studentEntity.RollNumber,
            ClassName = this.classEntity.Name,
            CourseName = this.courseEntity.Name
        }, enrolledBy, CancellationToken.None);

        Assert.Equal(this.studentEntity.StudentId, result.StudentId);
        Assert.Equal(this.classEntity.ClassId, result.ClassId);
        Assert.Equal(enrolledBy, result.EnrolledBy);
        Assert.Equal(this.courseEntity.CourseId, result.CourseId);
    }

    [Fact]
    public async Task UpdateEnrollmentAsync_WithValidRequest_UpdatesEnrollment()
    {
        var enrollment = new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = this.studentEntity.StudentId,
            ClassId = this.classEntity.ClassId,
            EnrolledByEmail = enrolledBy
        };
        this.dbContext.Enrollments.Add(enrollment);
        await this.dbContext.SaveChangesAsync();

        var enrollments = await this.dbContext.Enrollments.ToListAsync();

        var result = await this.enrollmentService.UpdateEnrollmentAsync(enrollment.EnrollmentId, new UpdateEnrollmentRequest
        {
            CourseId = this.courseEntity.CourseId
        }, CancellationToken.None);

        var updateEnrollment = await this.dbContext.Enrollments.SingleAsync();
        Assert.Equal(this.courseEntity.CourseId, updateEnrollment.CourseId);
    }

    [Fact]
    public async Task GetEnrollmentByIdAsync_WithExistingEnrollment_ReturnsEnrollmentDto()
    {
        var enrollment = new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = this.studentEntity.StudentId,
            ClassId = this.classEntity.ClassId,
            CourseId = this.courseEntity.CourseId,
            Student = this.studentEntity,
            Class = this.classEntity,
            Course = this.courseEntity,
            EnrolledByEmail = enrolledBy
        };
        this.dbContext.Enrollments.Add(enrollment);
        await this.dbContext.SaveChangesAsync();

        var result = await this.enrollmentService.GetEnrollmentByIdAsync(enrollment.EnrollmentId, CancellationToken.None);

        Assert.Equal(enrollment.EnrollmentId, result.EnrollmentId);
        Assert.Equal(this.studentEntity.StudentId, result.StudentId);
        Assert.Equal("Physics", result.ClassName);
        Assert.Equal("Science", result.CourseName);
    }
}
