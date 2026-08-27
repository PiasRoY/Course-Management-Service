namespace CourseManagement.Business.Services.Interfaces
{
    public interface IStorageService
    {
        FileStream OpenLocalFile(string fileName);
        Task<string> SaveStreamToLocalFile(Stream stream, string filePath);
    }
}