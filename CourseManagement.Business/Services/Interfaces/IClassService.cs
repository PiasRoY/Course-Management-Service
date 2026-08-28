using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IClassService
{
    Task<PageResult<ClassDto>> GetClassesAsync(PaginationParams @params, CancellationToken cancellationToken);
    Task<ClassDto> GetClassByNameAsync(string className, CancellationToken cancellationToken);
    Task<ClassDto> GetClassByIdAsync(Guid classId, CancellationToken cancellationToken);
    Task<IEnumerable<ClassDto>> GetClassesByInstructorEmailAsync(string email, CancellationToken cancellationToken);
    Task<PageResult<CourseDto>> GetCoursesByClassIdAsync(PaginationParams @params, Guid classId, CancellationToken cancellationToken);
    Task<PageResult<StudentDto>> GetStudentsByClassId(PaginationParams @params, Guid classId, CancellationToken cancellationToken);
    Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken);
    Task<ClassDto> UpdateClassByIdAsync(Guid classId, UpdateClassRequest updateClassRequest, CancellationToken cancellationToken);
    Task DeleteClassByIdAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken);
}
