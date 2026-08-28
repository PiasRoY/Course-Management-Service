using CourseManagement.Business.Services.Interfaces;

namespace CourseManagement.Business.Services;

public class StorageService : IStorageService
{
    private const string Bucket = "DummyS3Bucket";

    public StorageService()
    {
        Directory.CreateDirectory(Bucket);
    }

    public FileStream OpenLocalFile(string fileName)
    {
        var filePath = $"{Bucket}/{fileName}";
        return new FileStream(filePath, FileMode.Open);
    }

    public bool IsFileExistsLocally(string fileName)
    {
        var filePath = $"{Bucket}/{fileName}";
        return File.Exists(filePath);
    }

    public async Task<string> SaveStreamToLocalFile(Stream stream, string fileName)
    {
        var filePath = $"{Bucket}/{fileName}";
        using var fileStream = new FileStream(filePath, FileMode.CreateNew);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}
