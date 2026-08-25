using CourseManagement.Business.DTOs.EnrollmentDTOs;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentRequest createEnrollmentRequest, CancellationToken cancellationToken);
        Task DeleteEnrollmentAsync(DeleteEnrollmentRequest deleteEnrollmentRequest, CancellationToken cancellationToken);
        Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
        Task<EnrollmentDto> UpdateEnrollmentAsync(Guid enrollmentId, UpdateEnrollmentRequest updateEnrollmentRequest, CancellationToken cancellationToken);
    }
}