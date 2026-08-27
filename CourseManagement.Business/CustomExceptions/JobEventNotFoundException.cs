namespace CourseManagement.Business.CustomExceptions;

public class JobEventNotFoundException : NotFoundException
{
    public JobEventNotFoundException(Guid id) : 
        base($"JobEvent with id {id} was not found.") {}
}
