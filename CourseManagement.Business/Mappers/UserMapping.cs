using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Domain.Entities;
using System.Linq.Expressions;

namespace CourseManagement.Business.Mappers;

public static class UserMapping
{
    public static readonly Expression<Func<User, UserDto>> ProjectToUserDto = user =>
        new UserDto
        {
            UserId = user.UserId,
            EmailAddress = user.EmailAddress,
            FullName = $"{user.FirstName} {user.LastName}"
        };

    public static UserDto MapsToUserDto(User user) => ProjectToUserDto.Compile()(user);
}
