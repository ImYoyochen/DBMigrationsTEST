namespace MigrationsTEST.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileName);
        Task<byte[]?> GetFileAsync(string fileName);
        string GetFileUrl(string fileName);
    }
} 