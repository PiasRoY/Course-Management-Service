namespace CourseManagement.Business.CustomExceptions;

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(string courseName) :
        base($"Course named {courseName} was not found.") {}

    public CourseNotFoundException(Guid courseId) :
        base($"Course with id {courseId} was not found.")
    { }
}
