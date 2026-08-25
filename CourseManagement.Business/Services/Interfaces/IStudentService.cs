using CourseManagement.Business.DTOs.StudentsDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IStudentService
{
    Task<StudentDto> GetStudentByRollNoAsync(string studentRollNumber, CancellationToken cancellationToken);
    Task<StudentDto> CreateStudentByRollNoAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken);
    Task<StudentDto> UpdateStudentByRollNoAsync(string studentNumber, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken);
    Task DeleteStudentAsync(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken);
}
