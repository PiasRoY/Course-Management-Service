namespace CourseManagement.Business.CustomExceptions;

public class StudentNotFoundException : NotFoundException
{
    public StudentNotFoundException(string studentNumber)
        : base ($"Student with studentNumber {studentNumber} was not found.") {}

    public StudentNotFoundException(Guid studentId)
        : base($"Student with studentId {studentId} was not found.") { }

    public StudentNotFoundException(string email, string condition) 
        : base($"User : {email} with {condition} was not found.") {}
}
