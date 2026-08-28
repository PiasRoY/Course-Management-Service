using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PageResult<CourseDto>> GetCoursesAsync(PaginationParams @params, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseDto> CreateCourseAsync(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken);
        Task DeleteCourseByIdAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken);
        Task<CourseDto> UpdateCourseByIdAsync(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken);
        Task<PageResult<StudentDto>> GetStudentsByCourseAsync(PaginationParams @params, Guid courseId, CancellationToken cancellationToken);
        Task<PageResult<ClassDto>> GetClassesByCourseAsync(PaginationParams @params, Guid courseId, CancellationToken cancellationToken);
    }
}