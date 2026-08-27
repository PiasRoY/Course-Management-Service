using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface ITaskService
    {
        string EnqueueBulkImportUsersJob(JobEvent jobEvent);
        string? JobStatus(string jobId);
    }
}