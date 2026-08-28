using CourseManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace CourseManagement.Business.Services.Interfaces
{
    public interface IBulkService
    {
        Task<JobEvent> PreprocessingAsync(IFormFile formFile, CancellationToken cancellationToken);
        Task PostProcessingAsync(JobEvent jobEvent, string hangfireJobId, CancellationToken cancellationToken);
        Task ProcessBulkImportAsync<TRequest, TDto>(JobEvent jobEvent, Func<TRequest, CancellationToken, Task<TDto>> processRowAsync, CancellationToken cancellationToken);
        Task<FileStream> DownloadOutputCsvFileAsync(JobEvent jobEvent, CancellationToken cancellationToken);
    }
}