namespace MigrationsTEST.Services
{
    public class FileValidationService : IFileValidationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileValidationService> _logger;
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;
        private readonly Dictionary<string, byte[]> _fileSignatures;

        public FileValidationService(IConfiguration configuration, ILogger<FileValidationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // 從配置檔讀取允許的副檔名，預設值為常見的文件類型
            _allowedExtensions = configuration.GetSection("FileUpload:AllowedExtensions")
                .Get<string[]>() ?? new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
            
            // 從配置檔讀取最大檔案大小（預設 10MB）
            _maxFileSize = configuration.GetValue<long>("FileUpload:MaxFileSize", 10 * 1024 * 1024);

            // 初始化檔案標頭簽名
            _fileSignatures = new Dictionary<string, byte[]>
            {
                { ".jpg", new byte[] { 0xFF, 0xD8, 0xFF } },
                { ".jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
                { ".png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
                { ".pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 } },
                // 可以添加更多檔案類型的標頭簽名
            };
        }

        public bool ValidateFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var isAllowed = _allowedExtensions.Contains(extension);
            
            if (!isAllowed)
            {
                _logger.LogWarning($"Rejected file with unauthorized extension: {extension}");
            }
            
            return isAllowed;
        }

        public bool ValidateFileSize(long fileSize)
        {
            var isValid = fileSize > 0 && fileSize <= _maxFileSize;
            
            if (!isValid)
            {
                _logger.LogWarning($"Rejected file with invalid size: {fileSize} bytes");
            }
            
            return isValid;
        }

        public bool ValidateFileContent(Stream fileStream, string contentType)
        {
            try
            {
                // 檢查檔案是否為空
                if (fileStream.Length == 0)
                    return false;

                // 讀取檔案開頭的位元組來驗證檔案類型
                var headerBytes = new byte[8];
                fileStream.Position = 0;
                fileStream.Read(headerBytes, 0, headerBytes.Length);
                fileStream.Position = 0; // 重置串流位置

                // 根據內容類型檢查檔案標頭
                var extension = contentType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "application/pdf" => ".pdf",
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(extension) || !_fileSignatures.ContainsKey(extension))
                    return true; // 如果沒有對應的簽名，就跳過檢查

                var signature = _fileSignatures[extension];
                return headerBytes.Take(signature.Length).SequenceEqual(signature);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating file content: {ex.Message}");
                return false;
            }
        }
    }
} 