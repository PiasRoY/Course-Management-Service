using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services
{
    public interface ITaskService
    {
        string EnqueueBulkImportUsersJob(JobEvent jobEvent, CancellationToken cancellationToken);
        string? JobStatus(string jobId);
    }
}