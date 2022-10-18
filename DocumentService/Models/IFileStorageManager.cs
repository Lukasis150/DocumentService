using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Models
{
    public interface IFileStorageManager
    {
        public Task SaveFileAsync(string filePath, byte[] fileData);
        public Task<byte[]> GetFileAsync(string filePath);
        public Task UpdateFileAsync(string filePath, byte[] fileData);
    }
}
