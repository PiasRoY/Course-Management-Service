using CourseManagement.Business.CustomExceptions;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class JobEventService : IJobEventService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<JobEventService> logger;

    public JobEventService(ApplicationDbContext dbContext, ILogger<JobEventService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<JobEvent> GetJobEventByAsync(Guid jobEventId, CancellationToken cancellationToken)
    {
        var jobEvent = await this.dbContext
                                 .JobEvents
                                 .Where(j => j.JobEventId == jobEventId)
                                 .SingleOrDefaultAsync(cancellationToken);

        if (jobEvent == null)
        {
            throw new JobEventNotFoundException(jobEventId);
        }

        return jobEvent;
    }
}
