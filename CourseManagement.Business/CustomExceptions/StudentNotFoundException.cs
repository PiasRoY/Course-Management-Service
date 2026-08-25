namespace CourseManagement.Business.CustomExceptions;

public class StudentNotFoundException : NotFoundException
{
    public StudentNotFoundException(string studentNumber)
        : base ($"Student with studentNumber {studentNumber} was not found.") {}
}
