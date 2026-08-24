using CourseManagement.Business.DTOs.ClassDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IClassService
{
    Task<ClassDto> GetClassByNameAsync(string className, CancellationToken cancellationToken);
    Task<IEnumerable<ClassDto>> GetClassesByInstructorEmail(string email, CancellationToken cancellationToken);
    Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken);
    Task<ClassDto> UpdateClassByNameAsync(string className, UpdateClassRequest updateClassRequest, CancellationToken cancellationToken);
    Task DeleteClassByNameAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken);
}
