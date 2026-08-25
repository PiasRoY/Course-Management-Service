using CourseManagement.Business.DTOs.CourseDTOs;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken);
        Task DeleteCourseByNameAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseDto> UpdateCourseByIdAsync(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken);
    }
}