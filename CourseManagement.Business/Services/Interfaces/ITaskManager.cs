using CourseManagement.Business.DTOs.BulkImportDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ITaskManager
    {
        string? JobStatus(string jobId);
        Task BulkImportAsync(UserContextDto userContextDto, JobEvent jobEvent, ImportTypes importTypes);
        string EnqueueBulkImportJob(UserContextDto userContextDto, JobEvent jobEvent, ImportTypes importTypes);
    }
}