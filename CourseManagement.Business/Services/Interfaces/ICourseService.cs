using CourseManagement.Business.DTOs.CourseDTOs;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken);
        Task DeleteCourseByNameAsync(DeleteCourseRequest deleteCourseRequest, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken);
        Task<CourseDto> UpdateCourseByNameAsync(string courseName, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken);
    }
}