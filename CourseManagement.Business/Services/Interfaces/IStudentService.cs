using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IStudentService
{
    Task<PageResult<StudentDto>> GetStudentsAsync(PaginationParams @params, CancellationToken cancellationToken);
    Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken);
    Task<StudentDto> GetStudentByRollNoAsync(string studentRollNumber, CancellationToken cancellationToken);
    Task<StudentDto> CreateStudentAsync(CreateStudentRequest createStudentRequest, CancellationToken cancellationToken);
    Task<StudentDto> UpdateStudentByIdAsync(Guid studentId, UpdateStudentRequest updateStudentRequest, CancellationToken cancellationToken);
    Task DeleteStudentAsync(DeleteStudentRequest deleteStudentRequest, CancellationToken cancellationToken);
    Task<IEnumerable<ClassDto>> GetClassesByStudent(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<CourseDto>> GetCoursesByStudent(Guid userId, CancellationToken cancellationToken);
    Task<PageResult<StudentDto>> GetClassMatesByStudent(Guid userId, PaginationParams @params, CancellationToken cancellationToken);
    Task<PageResult<StudentDto>> GetCourseMatesByStudent(Guid userId, PaginationParams @params, CancellationToken cancellationToken);
}
