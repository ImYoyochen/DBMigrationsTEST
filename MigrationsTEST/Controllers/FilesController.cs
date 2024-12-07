using Microsoft.AspNetCore.Mvc;
using MigrationsTEST.Services;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MigrationsTEST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IFileValidationService _fileValidationService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IStorageService storageService,
            IFileValidationService fileValidationService,
            ILogger<FilesController> logger)
        {
            _storageService = storageService;
            _fileValidationService = fileValidationService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10MB limit
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                // 驗證檔案名稱（移除��在的危險字元）
                var sanitizedFileName = SanitizeFileName(file.FileName);
                
                // 驗證檔案大小
                if (!_fileValidationService.ValidateFileSize(file.Length))
                {
                    return BadRequest("File size exceeds the limit");
                }

                // 驗證檔案類型
                if (!_fileValidationService.ValidateFileExtension(sanitizedFileName))
                {
                    return BadRequest("File type not allowed");
                }

                // 驗證檔案內容
                using (var stream = file.OpenReadStream())
                {
                    if (!_fileValidationService.ValidateFileContent(stream, file.ContentType))
                    {
                        return BadRequest("Invalid file content");
                    }
                }

                string fileName = await _storageService.UploadFileAsync(file);
                var fileUrl = _storageService.GetFileUrl(fileName);

                // 計算檔案雜湊值（可選）
                string fileHash = await CalculateFileHashAsync(file);

                return Ok(new { fileName, fileUrl, fileHash });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                // 驗證檔案名稱
                if (!IsValidFileName(fileName))
                {
                    return BadRequest("Invalid file name");
                }

                var fileBytes = await _storageService.GetFileAsync(fileName);
                if (fileBytes == null)
                {
                    return NotFound();
                }

                // 設定安全相關的 headers
                Response.Headers.Add("X-Content-Type-Options", "nosniff");
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                string contentType = GetContentType(fileName);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving file: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            // 移除路徑中的非法��元
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            
            return Regex.Replace(fileName, invalidRegStr, "_");
        }

        private static bool IsValidFileName(string fileName)
        {
            // 檢查檔案名稱是否包含可疑的路徑字元
            return !fileName.Contains("..") && 
                   !fileName.Contains("/") && 
                   !fileName.Contains("\\") &&
                   !string.IsNullOrWhiteSpace(fileName) &&
                   fileName.Length <= 255;
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        private static async Task<string> CalculateFileHashAsync(IFormFile file)
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
} 