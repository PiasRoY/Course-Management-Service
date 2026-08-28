using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Domain.Entities;
using System.Linq.Expressions;
namespace CourseManagement.Business.Mappers;

public static class ClassMapper
{
    public static readonly Expression<Func<Class, ClassDto>> ProjectToClasDto = @class =>
        new ClassDto
        {
            ClassId = @class.ClassId,
            Name = @class.Name,
            Semester = @class.Semester,
            Calendar = @class.Calendar,
            SectionCode = @class.SectionCode,
            InstructorName = $"{@class.Instructor.FirstName} {@class.Instructor.LastName}",
            InstructorEmail = @class.Instructor.EmailAddress
        };

    public static ClassDto MapsToClassDto(Class @class, User instructor)
    {
        return new ClassDto
        {
            ClassId = @class.ClassId,
            Name = @class.Name,
            Semester = @class.Semester,
            Calendar = @class.Calendar,
            SectionCode = @class.SectionCode,
            InstructorName = $"{instructor.FirstName} {instructor.LastName}",
            InstructorEmail = instructor.EmailAddress
        };
    }

    public static Class MapsToClass(CreateClassRequest createClassRequest, User instructor)
    {
        return new Class
        {
            ClassId = Guid.NewGuid(),
            Name = createClassRequest.Name,
            Semester = createClassRequest.Semester,
            Year = createClassRequest.Year,
            SectionCode = createClassRequest.SectionCode,
            InstructorId = instructor.UserId
        };
    }

    public static void MapsStaticPropertiesToClass(UpdateClassRequest updateClassRequest, Class @class)
    {
        if (!string.IsNullOrWhiteSpace(updateClassRequest.ClassName)) 
        {
            @class.Name = updateClassRequest.ClassName;
        }

        if (updateClassRequest.Semester != null)
        {
            @class.Semester = updateClassRequest.Semester.Value;
        }

        if (updateClassRequest.Year != null)
        {
            @class.Year = (int) updateClassRequest.Year;
        }

        if (!string.IsNullOrWhiteSpace(updateClassRequest.SectionCode))
        {
            @class.SectionCode = updateClassRequest.SectionCode;
        }
    }
}
