namespace MigrationsTEST.Services
{
    public interface IFileValidationService
    {
        bool ValidateFileExtension(string fileName);
        bool ValidateFileSize(long fileSize);
        bool ValidateFileContent(Stream fileStream, string contentType);
    }
} 