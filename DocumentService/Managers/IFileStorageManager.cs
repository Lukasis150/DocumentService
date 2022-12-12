using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Managers
{
    public interface IFileStorageManager
    {
        public Task SaveFileAsync(string filePath, byte[] fileData);
        public Task<byte[]> GetFileAsync(string filePath);
        public Task UpdateFileAsync(string filePath, byte[] fileData);
    }
}
