using CourseManagement.Business.DTOs.ClassDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IClassService
{
    Task<ClassDto> GetClassByNameAsync(string className, CancellationToken cancellationToken);
    Task<ClassDto> GetClassByIdAsync(Guid classId, CancellationToken cancellationToken);
    Task<IEnumerable<ClassDto>> GetClassesByInstructorEmail(string email, CancellationToken cancellationToken);
    Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken);
    Task<ClassDto> UpdateClassByIdAsync(Guid classId, UpdateClassRequest updateClassRequest, CancellationToken cancellationToken);
    Task DeleteClassByIdAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken);
}
