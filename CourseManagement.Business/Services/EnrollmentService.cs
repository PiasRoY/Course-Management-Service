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
                         .GetItems(@params,
                                   e => EnrollmentMapper.MapsToEnrollmentDto(e),
                                   e => e.EnrollmentId,
                                   cancellationToken);
    }

    public async Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        Enrollment? enrollment = await GetEnrollmentAsync(enrollmentId, cancellationToken);

        if (enrollment == null)
        {
            throw new EnrollmentNotFoundException(enrollmentId);
        }

        return EnrollmentMapper.MapsToEnrollmentDto(enrollment);
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentRequest createEnrollmentRequest, CancellationToken cancellationToken)
    {
        if (await this.IsEnrollmentExists(createEnrollmentRequest.StudentId, createEnrollmentRequest.ClassId, createEnrollmentRequest.CourseId, cancellationToken))
        {
            throw new InvalidOperationException("Student is already enrolled to this class, course combination.");
        }

        var enrollment = EnrollmentMapper.MapsToEnrollment(createEnrollmentRequest);

        await this.dbContext.AddAsync(enrollment, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("New enrollment {Student}, {Course}, {Class} is created.", enrollment.StudentId, enrollment.CourseId, enrollment.ClassId);

        return EnrollmentMapper.MapsToEnrollmentDto(enrollment);
    }

    public async Task<EnrollmentDto> UpdateEnrollmentAsync(Guid enrollmentId, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken)
    {
        var enrollment = await this.GetEnrollmentAsync(enrollmentId, cancellationToken);

        if (enrollment == null)
        {
            throw new EnrollmentNotFoundException(enrollmentId);
        }

        EnrollmentMapper.UpdateEnrollment(enrollment, updateEnrollmentRequest);

        if (!await this.IsEnrollmentExists(enrollment.EnrollmentId, enrollment.ClassId, enrollment.CourseId, cancellationToken))
        {
            throw new InvalidOperationException("Student is already enrolled to this class, course combination.");
        }

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

    private async Task<Enrollment?> GetEnrollmentAsync(Guid studentId, Guid classId, Guid? courseId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Enrollments
                         .Where(e => e.StudentId == studentId)
                         .Where(e => e.ClassId == classId)
                         .Where(e => e.CourseId == courseId)
                         .FirstOrDefaultAsync(cancellationToken);
    }
    private async Task<Enrollment?> GetEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        return await this.dbContext
                         .Enrollments
                         .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId, cancellationToken);
    }
}
