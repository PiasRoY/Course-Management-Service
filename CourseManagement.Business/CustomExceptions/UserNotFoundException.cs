namespace CourseManagement.Business.CustomExceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string email) : base($"User with email : {email} was not found") {}
    public UserNotFoundException(Guid userId) : base($"User with id : {userId} was not found") {}
}
