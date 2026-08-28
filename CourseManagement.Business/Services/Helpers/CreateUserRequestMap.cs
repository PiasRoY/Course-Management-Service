using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CsvHelper.Configuration;

namespace CourseManagement.Business.Services.Helpers;

public class CreateUserRequestMap : ClassMap<CreateUserRequest>
{
    public CreateUserRequestMap()
    {
        Map(m => m.EmailAddress);
        Map(m => m.Password);
        Map(m => m.FirstName);
        Map(m => m.LastName);
        Map(m => m.Roles)
            .Convert(csv =>
            {
                var rolesField = csv.Row.GetField("Roles");

                if (string.IsNullOrWhiteSpace(rolesField))
                {
                    return new List<UserRoles>();
                }

                return rolesField.Split("|")
                                 .Select(r => Enum.Parse<UserRoles>(r, true))
                                 .ToList();
            });
    }
}
