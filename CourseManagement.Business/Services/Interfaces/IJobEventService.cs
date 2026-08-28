using CourseManagement.Domain.Entities;

namespace CourseManagement.Business.Services
{
    public interface IJobEventService
    {
        Task<JobEvent> GetJobEventByAsync(Guid jobEventId, CancellationToken cancellationToken);
    }
}