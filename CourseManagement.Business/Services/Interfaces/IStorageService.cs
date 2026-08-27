namespace CourseManagement.Business.Services.Interfaces
{
    public interface IStorageService
    {
        bool IsFileExistsLocally(string fileName);
        FileStream OpenLocalFile(string fileName);
        Task<string> SaveStreamToLocalFile(Stream stream, string filePath);
    }
}