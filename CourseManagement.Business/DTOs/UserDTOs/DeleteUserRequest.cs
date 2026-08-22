namespace CourseManagement.Business.DTOs.UserDTOs;

public class DeleteUserRequest
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
}
