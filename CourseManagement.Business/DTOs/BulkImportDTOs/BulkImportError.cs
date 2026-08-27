using System.Diagnostics.CodeAnalysis;

namespace CourseManagement.Business.DTOs.BulkImportDTOs;

public class BulkImportError<T>
{
    public required T Item { get; set; }
    public string? ErrorMessage { get; set; }

    [SetsRequiredMembers]
    public BulkImportError(T item, string errorMessage)
    {
        this.Item = item;
        this.ErrorMessage = errorMessage;
    }
}
