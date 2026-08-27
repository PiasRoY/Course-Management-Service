using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services
{
    public interface ITaskService
    {
        string EnqueueBulkImportUsersJob(JobEvent jobEvent);
        string? JobStatus(string jobId);
    }
}