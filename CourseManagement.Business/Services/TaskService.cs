using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using Hangfire;
using Hangfire.Storage;

namespace CourseManagement.Business.Services;

public class TaskService : ITaskService
{
    private readonly IMonitoringApi monitoringApi;

    public TaskService(IMonitoringApi monitoringApi)
    {
        this.monitoringApi = monitoringApi;
    }

    public string EnqueueBulkImportUsersJob(JobEvent jobEvent)
    {
        return BackgroundJob.Enqueue<IBulkService>(
            bulkService => bulkService.ProcessBulkImportUsersAsync(jobEvent, CancellationToken.None));
    }

    public string? JobStatus(string jobId)
    {
        return monitoringApi.JobDetails(jobId).History.FirstOrDefault()?.StateName;
    }
}
