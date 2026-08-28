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

    public static List<EnrollmentDto> MapsToEnrollmentDtoList(List<Enrollment> enrollments)
    {
        var enrollmentDtos = new List<EnrollmentDto>();

        foreach (var enrollment in enrollments)
        {
            enrollmentDtos.Add(MapsToEnrollmentDto(enrollment));
        }

        return enrollmentDtos;
    }

    public static EnrollmentCourseDto MapsToEnrollmentCourseDto(Enrollment enrollment)
    {
        return new EnrollmentCourseDto
        {
            CourseId = enrollment.CourseId!.Value,
            StudentId = enrollment.StudentId,
            EnrolledBy = enrollment.CreatedBy,
            EnrolledAt = enrollment.CreatedAt
        };
    }

    public static Enrollment MapsToEnrollment(CreateEnrollmentByClassRequest createEnrollmentRequest)
    {
        return new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = createEnrollmentRequest.StudentId,
            CourseId = null,
            ClassId = createEnrollmentRequest.ClassId,
        };
    }

    public static List<Enrollment> MapsToEnrollmentList(CreateEnrollmentByCourseRequest request, List<Guid> classIdList)
    {
        var enrollmentList = new List<Enrollment>();

        foreach (var classId in classIdList)
        {
            enrollmentList.Add(new Enrollment
            {
                EnrollmentId = Guid.NewGuid(),
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                ClassId = classId
            });
        }

        return enrollmentList;
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
