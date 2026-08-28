using CourseManagement.Business.Enums;
using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ITaskManager
    {
        string? JobStatus(string jobId);
        Task BulkImportAsync(JobEvent jobEvent, ImportTypes importTypes);
        string EnqueueBulkImportUsersJob(JobEvent jobEvent, ImportTypes importTypes);
    }
}