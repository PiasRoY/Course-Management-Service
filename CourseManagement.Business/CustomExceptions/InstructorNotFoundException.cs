namespace CourseManagement.Business.CustomExceptions;

public class InstructorNotFoundException : NotFoundException
{
    public InstructorNotFoundException(string email) 
        : base($"Instructor with email ({email} was not found.)") {}

    public InstructorNotFoundException(Guid id)
        : base($"Instructor with id ({id}) was not found.") {}
}
