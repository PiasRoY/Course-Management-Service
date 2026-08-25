using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Domain.Entities;
namespace CourseManagement.Business.Mappers;

public static class ClassMapper
{
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
