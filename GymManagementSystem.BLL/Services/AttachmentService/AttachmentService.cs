using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly string[] _allowedExtenstions = { ".jpg", ".jpeg", ".png" };

        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead)
                return null;
            if (fileStream.Length == 0)
                return null;
            if(fileStream.Length>_maxFileSize)
            {
                _logger.LogWarning($"Rejected Upload: File too large ({fileStream.Length}) Bytes");
                return null;
            }

            var extension = Path.GetExtension(fileName);
            if(string.IsNullOrEmpty(extension) || !_allowedExtenstions.Contains(extension))
            {
                _logger.LogWarning($"Rejected Upload: Extension {extension} not allowed");
                return null;
            }

            var folderPath = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(folderPath);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, storedFileName);

            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(fs, ct);
                return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload file {fileName}");
                return null;
            }

        }
        public bool Delete(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
                return false;
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, folderName, fileName);
                if (!File.Exists(filePath))
                    return false;
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete attachment {fileName}");
                return false;
            }
        }

        public (Stream Stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileName))
                return null;
            var filePath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            if (!File.Exists(filePath))
                return null;

            var contentType = Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/cotet-stream"
            };

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, contentType);
        }
    }
}
