using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.DTOs.PaginationDTOs;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<PageResult<EnrollmentDto>> GetEnrollmentsAsync(PaginationParams @params, CancellationToken cancellationToken);
        Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentRequest createEnrollmentRequest, CancellationToken cancellationToken);
        Task DeleteEnrollmentAsync(DeleteEnrollmentRequest deleteEnrollmentRequest, CancellationToken cancellationToken);
        Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
        Task<EnrollmentDto> UpdateEnrollmentAsync(Guid enrollmentId, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken);
    }
}