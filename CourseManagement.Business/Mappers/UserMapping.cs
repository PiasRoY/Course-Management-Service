using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Mappers;

public static class UserMapping
{
    public static UserDto MapsToUserDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            EmailAddress = user.EmailAddress,
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
}
