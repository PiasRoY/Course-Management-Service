using CourseManagement.Business.DTOs.ClassDTOs;

namespace CourseManagement.Business.Services.Interfaces;

public interface IClassService
{
    Task<ClassDto> CreateClassAsync(CreateClassRequest createClassRequest, CancellationToken cancellationToken);
    Task<ClassDto> UpdateClassByNameAsync(UpdateClassRequest updateClassRequest, CancellationToken cancellationToken);
    Task DeleteClassByNameAsync(DeleteClassRequest deleteClassRequest, CancellationToken cancellationToken);
}
