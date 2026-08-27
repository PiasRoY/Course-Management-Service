using CourseManagement.Business.Services.Interfaces;

namespace CourseManagement.Business.Services;

public class StorageService : IStorageService
{
    private const string Bucket = "DummyS3Bucket";

    public StorageService()
    {
    }

    public FileStream OpenLocalFile(string fileName)
    {
        return new FileStream(fileName, FileMode.Open);
    }

    public async Task<string> SaveStreamToLocalFile(Stream stream, string fileName)
    {
        var filePath = $"{Bucket}/{fileName}";
        using var fileStream = new FileStream(filePath, FileMode.CreateNew);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}
