using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.DTOs.BulkImportDTOs;

public class StatusResult
{
    public required BulkProcessStatus Status { get; set; }
    public FileStream? OutputFile { get; set; }
}
