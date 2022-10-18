using Microsoft.AspNetCore.Mvc;
using System.IO.Abstractions;

namespace DocumentService.Models
{
    public class HddManager : IFileStorageManager
    {
        private readonly ILogger<HddManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _basePath;
        private readonly IFileSystem _fileWrapper;

        public HddManager(ILogger<HddManager> logger, IConfiguration configuration, IFileSystem fileWrapper)
        {
            _logger = logger;
            _configuration = configuration;
            _basePath = configuration["FileStorage:HDD:BasePath"];
            _fileWrapper = fileWrapper;

            if (_basePath == null)
            {
                _logger.LogInformation("Your base path is null");
            }
        }

        public async Task<byte[]> GetFileAsync(string fileName)
        {
            string path = Path.Combine(_basePath, fileName);
            if (_fileWrapper.File.Exists(path))
            {
                return _fileWrapper.File.ReadAllBytes(path);
            }
            else
            {
                throw new Exception("File does not exists on HDD.");
            }
        }

        public async Task SaveFileAsync(string fileName, byte[] fileData)
        {
            string path = Path.Combine(_basePath, fileName);
            if (!_fileWrapper.File.Exists(path))
            {
                await _fileWrapper.File.WriteAllBytesAsync(path, fileData);
                _logger.LogInformation("File is saved on HDD.");
            }
            else
            {
                throw new Exception("File with this id exist on HDD. For updating use PUT method.");
            }
        }

        public async Task UpdateFileAsync(string fileName, byte[] fileData)
        {
            string path = Path.Combine(_basePath, fileName);
            if (_fileWrapper.File.Exists(path))
            {
                await _fileWrapper.File.WriteAllBytesAsync(path, fileData);
                _logger.LogInformation("File is updated.");
            }
            else
            {
                throw new Exception("File does not exists on HDD.");
            }
        }
    }
}
