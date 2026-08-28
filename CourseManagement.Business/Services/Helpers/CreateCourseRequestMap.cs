using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.Enums;
using CsvHelper.Configuration;

namespace CourseManagement.Business.Services.Helpers;

public class CreateCourseRequestMap : ClassMap<CreateCourseRequest>
{
    public CreateCourseRequestMap()
    {
        Map(m => m.Name);
        Map(m => m.Title);
        Map(m => m.Credits);
        Map(m => m.ClassNames)
            .Convert(csv =>
            {
                var rolesField = csv.Row.GetField("Roles");

                if (string.IsNullOrWhiteSpace(rolesField))
                {
                    return [];
                }

                return [.. rolesField.Split("|")];
            });
    }
}
