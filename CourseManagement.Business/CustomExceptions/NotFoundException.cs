namespace CourseManagement.Business.CustomExceptions;

public abstract class NotFoundException : Exception 
{
    public NotFoundException(string message) : base(message)
    {}
}
