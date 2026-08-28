using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.Services;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourseManagement.UnitTests.Services.Tests;

public class CourseServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly CourseService courseService;
    private readonly Class classEntity;
    private readonly Course course;

    public CourseServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());

        this.classEntity = new Class
        {
            ClassId = Guid.NewGuid(),
            Name = "Physics",
            Semester = Semester.Fall,
            Year = 2026,
            SectionCode = "A"
        };

        this.course = new Course
        {
            CourseId = Guid.NewGuid(),
            Name = "Science",
            Title = "Introduction to Science",
            Credits = 3,
            CourseClasses = [new CourseClass { Class = this.classEntity, ClassId = this.classEntity.ClassId }]
        };
        
        this.dbContext.Courses.Add(this.course);
        this.dbContext.SaveChanges();
        
        this.courseService = new CourseService(this.dbContext, Mock.Of<ILogger<CourseService>>());
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task CreateCourseAsync_WithValidRequest_CreatesCourseAndReturnsCourseDto()
    {
        var result = await this.courseService.CreateCourseAsync(new CreateCourseRequest
        {
            Name = "Chemistry",
            Title = "Introduction to Science",
            Credits = 3,
            ClassNames = [this.classEntity.Name]
        }, CancellationToken.None);

        Assert.Equal("Chemistry", result.Name);
        Assert.Single(result.ClassNames, "Physics");
        Assert.Equal(2, await this.dbContext.Courses.CountAsync());
    }

    [Fact]
    public async Task UpdateCourseByIdAsync_WithValidRequest_UpdatesCourseAndReturnsCourseDto()
    {
        var result = await this.courseService.UpdateCourseByIdAsync(this.course.CourseId, new UpdateCourseRequest
        {
            Name = "Advanced Science",
            Title = "Advanced Science Topics",
            Credits = 4
        }, CancellationToken.None);

        Assert.Equal("Advanced Science", result.Name);
        Assert.Equal("Advanced Science Topics", result.Title);
        Assert.Equal(4, result.Credits);
    }

    [Fact]
    public async Task GetCourseByIdAsync_WithExistingCourse_ReturnsCourseDto()
    {
        var result = await this.courseService.GetCourseByIdAsync(this.course.CourseId, CancellationToken.None);

        Assert.Equal(this.course.CourseId, result.CourseId);
        Assert.Equal("Science", result.Name);
        Assert.Contains("Physics", result.ClassNames);
    }
}
