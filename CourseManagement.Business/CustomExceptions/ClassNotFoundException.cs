namespace CourseManagement.Business.CustomExceptions;

public class ClassNotFoundException : NotFoundException
{
    public ClassNotFoundException(string Name) : 
        base($"Class named {Name} is not found.") {}

    public ClassNotFoundException(Guid Id) :
        base($"Class with id {Id} is not found.") {}
}
