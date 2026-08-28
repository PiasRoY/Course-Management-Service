using CourseManagement.Business.CustomExceptions;
using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.Extensions;
using CourseManagement.Business.Mappers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<EnrollmentService> logger;

    public EnrollmentService(ApplicationDbContext dbContext, ILogger<EnrollmentService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<PageResult<EnrollmentDto>> GetEnrollmentsAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Enrollments
                         .OrderBy(e => e.CreatedAt)
                         .ThenBy(e => e.EnrollmentId)
                         .Select(EnrollmentMapper.ProjectToEnrollmentDto)
                         .GetItemsAsync(@params, cancellationToken);
    }

    public async Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await this.dbContext
                                   .Enrollments
                                   .Where(e => e.EnrollmentId == enrollmentId)
                                   .Select(EnrollmentMapper.ProjectToEnrollmentDto)
                                   .SingleOrDefaultAsync(cancellationToken);

        return enrollment ?? throw new EnrollmentNotFoundException(enrollmentId);
    }

    public async Task<EnrollmentDto> CreateEnrollmentByClassAsync(CreateEnrollmentByClassRequest request, CancellationToken cancellationToken)
    {
        if (await this.IsEnrollmentExists(request.StudentId, request.ClassId, null, cancellationToken))
        {
            throw new InvalidOperationException("Student is already enrolled to this class, course combination.");
        }

        var enrollment = EnrollmentMapper.MapsToEnrollment(request);

        await this.dbContext.AddAsync(enrollment, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New enrollment {Student}, {Course}, {Class} is created.", enrollment.StudentId, enrollment.CourseId, enrollment.ClassId);

        return EnrollmentMapper.MapsToEnrollmentDto(enrollment);
    }

    public async Task<EnrollmentCourseDto> CreateEnrollmentByCourseAsync(CreateEnrollmentByCourseRequest request, CancellationToken cancellationToken)
    {
        var courseClasses = this.dbContext
                                .CourseClasses
                                .Where(cc => cc.CourseId == request.CourseId)
                                .Select(cc => cc.ClassId);

        var enrolledClasses = this.dbContext
                                  .Enrollments
                                  .Where(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId)
                                  .Select(cc => cc.ClassId);

        var remainingClasses = await courseClasses.Except(enrolledClasses)
                                                  .ToListAsync(cancellationToken);

        if (remainingClasses.Count == 0)
        {
            throw new InvalidOperationException($"Student {request.StudentId} has already been enrolled to all the classes of the course {request.CourseId}");
        }

        var enrollments = EnrollmentMapper.MapsToEnrollmentList(request, remainingClasses);

        await this.dbContext.AddRangeAsync(enrollments, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Student {Id} got enrolled to the course {Id}.", request.StudentId, request.CourseId);

        return EnrollmentMapper.MapsToEnrollmentCourseDto(enrollments.First());
    }

    public async Task<EnrollmentDto> CreateEnrollmentByClassNamesAsync(CreateEnrollmentByClassNames request, CancellationToken cancellationToken)
    {
        var studentId = await this.dbContext.Students.AsNoTracking().Where(s => s.RollNumber == request.StudentRollNumber).Select(s => s.StudentId).SingleOrDefaultAsync(cancellationToken);
        var classId = await this.dbContext.Classes.AsNoTracking().Where(cl => cl.Name == request.ClassName).Select(cl => cl.ClassId).SingleOrDefaultAsync(cancellationToken);

        Guid? courseId = null;
        if (string.IsNullOrWhiteSpace(request.CourseName))
        {
            courseId = await this.dbContext.Courses.AsNoTracking().Where(c => c.Name == request.CourseName).Select(c => c.CourseId).SingleOrDefaultAsync(cancellationToken);
        }

        if (await this.IsEnrollmentExists(studentId, classId, courseId, cancellationToken))
        {
            throw new InvalidOperationException("Student is already enrolled to this class, course combination.");
        }

        var enrollment = new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            ClassId = classId
        };

        await this.dbContext.AddAsync(enrollment, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New enrollment {Student}, {Course}, {Class} is created.", enrollment.StudentId, enrollment.CourseId, enrollment.ClassId);

        return EnrollmentMapper.MapsToEnrollmentDto(enrollment);
    }

    public async Task<EnrollmentDto> UpdateEnrollmentAsync(Guid enrollmentId, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken)
    {
        var enrollment = await this.dbContext
                                   .Enrollments
                                   .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId, cancellationToken);

        if (enrollment == null)
        {
            throw new EnrollmentNotFoundException(enrollmentId);
        }

        if (!await this.IsEnrollmentExists(enrollment.StudentId, enrollment.ClassId, enrollment.CourseId, cancellationToken))
        {
            throw new InvalidOperationException("Student is already enrolled to this class, course combination.");
        }

        EnrollmentMapper.UpdateEnrollment(enrollment, updateEnrollmentRequest);

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Enrollment with id {Id} is updated.", enrollment.EnrollmentId);

        return EnrollmentMapper.MapsToEnrollmentDto(enrollment);
    }

    public async Task DeleteEnrollmentAsync(DeleteEnrollmentRequest deleteEnrollmentRequest, CancellationToken cancellationToken)
    {
        await this.dbContext
                  .Enrollments
                  .Where(e => e.EnrollmentId == deleteEnrollmentRequest.EnrollmentId)
                  .ExecuteDeleteAsync(cancellationToken);

        this.logger.LogInformation("Enrollment with id {Id} is deleted.", deleteEnrollmentRequest.EnrollmentId);
    }

    private async Task<bool> IsEnrollmentExists(Guid studentId, Guid classId, Guid? courseId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Enrollments
                         .Where(e => e.StudentId == studentId)
                         .Where(e => e.ClassId == classId)
                         .Where(e => e.CourseId == courseId)
                         .AnyAsync(cancellationToken);
    }
}
