namespace CourseManagement.Business.CustomExceptions;

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(string courseName) :
        base($"Course named {courseName} was not found.") {}
}
