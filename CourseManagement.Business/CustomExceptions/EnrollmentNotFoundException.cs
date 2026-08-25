namespace CourseManagement.Business.CustomExceptions;

public class EnrollmentNotFoundException : NotFoundException
{
    public EnrollmentNotFoundException(Guid enrollmentId) :
        base($"Enrollment with id {enrollmentId} was not found.") {}
}
