namespace CourseManagement.Business.CustomExceptions;

public class ClassNotFoundException : NotFoundException
{
    public ClassNotFoundException(string Name) : 
        base($"Class named {Name} was not found.") {}

    public ClassNotFoundException(Guid Id) :
        base($"Class with id {Id} was not found.") {}

    public ClassNotFoundException(IEnumerable<string> classNames) :
        base($"Classes named {string.Join(",", classNames)} were not found.") {}
}
