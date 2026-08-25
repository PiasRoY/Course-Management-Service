using CourseManagement.Business.DTOs.StudentsDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IStudentService
{
    Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken);
    Task<StudentDto> GetStudentByRollNoAsync(string studentRollNumber, CancellationToken cancellationToken);
    Task<StudentDto> CreateStudentAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken);
    Task<StudentDto> UpdateStudentByIdAsync(Guid studentId, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken);
    Task DeleteStudentAsync(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken);
}
