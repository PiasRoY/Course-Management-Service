using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Domain.Entities;
using System.Linq.Expressions;

namespace CourseManagement.Business.Mappers;

public static class EnrollmentMapper
{
    public static readonly Expression<Func<Enrollment, EnrollmentDto>> ProjectToEnrollmentDto = enrollment =>
        new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            StudentEmail = enrollment.Student != null ?
                           (enrollment.Student.User == null ? null : enrollment.Student.User.EmailAddress) : null,
            ClassId = enrollment.ClassId,
            ClassName = enrollment.Class != null ? enrollment.Class.Name : null,
            CourseId = enrollment.CourseId,
            CourseName = enrollment.Course != null ? enrollment.Course.Name : null,
            EnrolledAt = enrollment.CreatedAt,
            EnrolledBy = enrollment.CreatedBy
        };

    public static EnrollmentDto MapsToEnrollmentDto(Enrollment enrollment) => ProjectToEnrollmentDto.Compile()(enrollment);

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
