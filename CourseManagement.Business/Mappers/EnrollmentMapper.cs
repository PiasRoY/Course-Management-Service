using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Mappers;

public static class EnrollmentMapper
{
    public static EnrollmentDto MapsToEnrollmentDto(Enrollment enrollment)
    {
        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            ClassId = enrollment.ClassId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.CreatedAt,
            EnrolledBy = enrollment.CreatedBy
        };
    }

    public static Enrollment MapsToEnrollment(CreateEnrollmentRequest createEnrollmentRequest)
    {
        return new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = createEnrollmentRequest.StudentId,
            CourseId = createEnrollmentRequest.CourseId,
            ClassId = createEnrollmentRequest.ClassId,
        };
    }

    public static void UpdateEnrollment(Enrollment enrollment, UpdateEnrollmentRequest updateEnrollmentRequest)
    {
        if (updateEnrollmentRequest.StudentId != null)
        {
            enrollment.StudentId = updateEnrollmentRequest.StudentId.Value;
        }

        if (updateEnrollmentRequest.ClassId != null)
        {
            enrollment.ClassId = updateEnrollmentRequest.ClassId.Value;
        }

        if (updateEnrollmentRequest.CourseId != null)
        {
            enrollment.CourseId = updateEnrollmentRequest.CourseId.Value;
        }
    }
}
