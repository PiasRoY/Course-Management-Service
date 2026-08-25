using CourseManagement.Business.DTOs.CourseDTOs;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseDto> CreateCourseAsync(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken);
        Task DeleteCourseByIdAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken);
        Task<CourseDto> UpdateCourseByIdAsync(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken);
    }
}