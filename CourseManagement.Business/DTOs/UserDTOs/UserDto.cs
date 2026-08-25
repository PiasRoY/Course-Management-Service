namespace CourseManagement.Business.DTOs.UserDTOs;

public class UserDto
{
    required public Guid UserId { get; set; }
    required public string EmailAddress { get; set; } 
    required public string FullName { get; set; }
}
