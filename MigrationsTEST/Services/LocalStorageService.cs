using Microsoft.Extensions.Configuration;

namespace MigrationsTEST.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _storageDirectory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(IConfiguration configuration, ILogger<LocalStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                // 生成唯一的檔案名稱
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(_storageDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded successfully: {fileName}");
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_storageDirectory, fileName);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation($"File deleted successfully: {fileName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]?> GetFileAsync(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_storageDirectory, fileName);
                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving file: {ex.Message}");
                throw;
            }
        }

        public string GetFileUrl(string fileName)
        {
            // 本地存儲返回相對路徑
            return $"/api/files/{fileName}";
        }
    }
} 