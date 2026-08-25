using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Mappers;

public static class CourseMapper
{
    public static Course MapsToCourse(CreateCourseRequest createCourseRequest, IEnumerable<Guid> classIds)
    {
        var courseId = Guid.NewGuid();

        return new Course
        {
            CourseId = courseId,
            Name = createCourseRequest.Name,
            Title = createCourseRequest.Title,
            Credits = createCourseRequest.Credits,
            CourseClasses = classIds.Select(classId => new CourseClass
            {
                CourseId = courseId,
                ClassId = classId
            }).ToList()
        };
    }

    public static CourseDto MapsToCourseDto(Course course, IEnumerable<string> classNames)
    {
        return new CourseDto
        {
            CourseId = course.CourseId,
            Name = course.Name,
            Title = course.Title,
            Credits = course.Credits,
            ClassNames = classNames
        };
    }

    public static void UpdateCourseFromCourseDto(Course course, UpdateCourseRequest updateCourseRequest, IEnumerable<Guid> classIds)
    {
        if (!string.IsNullOrEmpty(updateCourseRequest.Name))
        {
            course.Name = updateCourseRequest.Name;
        }

        if (!string.IsNullOrEmpty(updateCourseRequest.Title))
        {
            course.Title = updateCourseRequest.Title;
        }

        if (updateCourseRequest.Credits != null)
        {
            course.Credits = (int) updateCourseRequest.Credits;
        }

        if (updateCourseRequest.ClassNames != null)
        {
            course.CourseClasses = classIds.Select(classId => new CourseClass
            {
                CourseId = course.CourseId,
                ClassId = classId
            }).ToList();
        }
    }
}
