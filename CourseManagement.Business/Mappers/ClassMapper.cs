using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.Mappers;

public static class ClassMapper
{
    public static ClassDto MapsToClassDto(Class @class, User instructor)
    {
        return new ClassDto
        {
            Name = @class.Name,
            Semester = @class.Semester,
            Calendar = @class.Calendar,
            SectionCode = @class.SectionCode,
            InstructorName = $"{instructor.FirstName} {instructor.LastName}",
            InstructorEmail = instructor.EmailAddress
        };
    }

    public static void MapsStaticPropertiesToClass(UpdateClassRequest updateClassRequest, Class @class)
    {
        if (updateClassRequest.Semester != null)
        {
            @class.Semester = (Semester) updateClassRequest.Semester;
        }

        if (updateClassRequest.Year != null)
        {
            @class.Year = (int) updateClassRequest.Year;
        }

        if (!string.IsNullOrEmpty(updateClassRequest.SectionCode))
        {
            @class.SectionCode = updateClassRequest.SectionCode;
        }
    }
}
