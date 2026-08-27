using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Domain.Entities;

public class JobEvent : BaseAuditEntity
{
    public Guid JobEventId { get; set; }
    public string HangfireJobId { get; set; } = "";
    public required BulkProcessStatus JobEventStatus { get; set; }
    public required string InputFilePath { get; set; }
    public string OutputFilePath { get; set; } = "";
}
