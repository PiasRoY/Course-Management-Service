namespace CourseManagement.Business.Services.Helpers
{
    public interface ICsvFileHelper
    {
        IAsyncEnumerable<TOut> ReadRecordsAsync<TOut>(Stream file, CancellationToken cancellationToken);
        Task<Stream> WriteRecordsAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken);
    }
}