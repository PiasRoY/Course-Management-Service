using CsvHelper;
using System.Globalization;

namespace CourseManagement.Business.Services.Helpers;

public class CsvFileHelper : ICsvFileHelper
{
    public async IAsyncEnumerable<T> ReadRecordsAsync<T>(Stream file, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(file);
        using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        
        await foreach (var item in csv.GetRecordsAsync<T>(cancellationToken))
        {
            yield return item;
        }
    }

    public async Task<Stream> WriteRecordsAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken)
    {
        var file = new MemoryStream();

        using (var streamWriter = new StreamWriter(file, leaveOpen: true))
        using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true))
        {
            await csv.WriteRecordsAsync(items, cancellationToken);
        }

        file.Position = 0;

        return file;
    }
}
