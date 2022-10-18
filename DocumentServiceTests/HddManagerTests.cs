using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using DocumentServiceTests.Mocks;
using DocumentService.Models;

namespace DocumentServiceTests
{
    public class HddManagerTests
    {
        private readonly IConfiguration _configuration;
        private readonly string _basePath;

        public HddManagerTests()
        {
            var configMock = new ConfigurationMock();
            _configuration = configMock.GetConfig();
            _basePath = _configuration["FileStorage:HDD:BasePath"];
        }

        [Fact]
        public async void SaveFileWorks()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile = new MockFileData("line1\nline2\nline3");
            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName = "mockFile.txt";

            await storageManager.SaveFileAsync(fileName, mockFile.Contents);

            Assert.True(mockFileSystem.FileExists(_basePath + "/" + fileName));
        }

        [Fact]
        public async void SaveFileWorks_ThrowExceptionIfFileExists()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile = new MockFileData("line1\nline2\nline3");
            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName = "mockFile.txt";

            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName), mockFile.Contents);

            await Assert.ThrowsAsync<Exception>(() => storageManager.SaveFileAsync(fileName, mockFile.Contents));
        }

        [Fact]
        public async void GetFileWorks()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile1 = new MockFileData("line1\nline2\nline3");
            var mockFile2 = new MockFileData("line1\nline2\nline3\nline4");
            var mockFile3 = new MockFileData("line1\nline2\nline3\nline4\nline5");

            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName1 = "mockFile1.txt";
            string fileName2 = "mockFile2.txt";
            string fileName3 = "mockFile3.txt";

            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName1), mockFile1.Contents);
            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName2), mockFile2.Contents);
            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName3), mockFile3.Contents);

            byte[] fileFromStorage = await storageManager.GetFileAsync(fileName1);

            Assert.True(fileFromStorage.SequenceEqual(mockFile1.Contents));
        }

        [Fact]
        public async void GetFile_ThrowsExceptionIfFileDoesNotExists()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile2 = new MockFileData("line1\nline2\nline3\nline4");
            var mockFile3 = new MockFileData("line1\nline2\nline3\nline4\nline5");

            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName1 = "mockFile1.txt";
            string fileName2 = "mockFile2.txt";
            string fileName3 = "mockFile3.txt";

            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName2), mockFile2.Contents);
            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName3), mockFile3.Contents);

            Exception ex = await Assert.ThrowsAsync<Exception>(() => storageManager.GetFileAsync(fileName1));
            Assert.Equal("File does not exists on HDD.", ex.Message);
        }

        [Fact]
        public async void UpdateFileWorks()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile1 = new MockFileData("line1\nline2\nline3");
            var mockFile2 = new MockFileData("line1\nline2\nline3\nline4");

            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName1 = "mockFile1.txt";

            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName1), mockFile1.Contents);

            await storageManager.UpdateFileAsync(fileName1, mockFile2.Contents);

            byte[] updatedFile = mockFileSystem.File.ReadAllBytes(Path.Combine(_basePath, fileName1));

            Assert.True(updatedFile.SequenceEqual(mockFile2.Contents));
        }

        [Fact]
        public async void UpdateFile_ThrowsExceptionIfFileDoesNotExists()
        {
            var mockFileSystem = CreateFileSystem();

            var mockFile1 = new MockFileData("line1\nline2\nline3");
            var mockFile2 = new MockFileData("line1\nline2\nline3\nline4");
            var mockFile3 = new MockFileData("line1\nline2\nline3\nline4\nline5");

            var loggerMock = new Mock<ILogger<HddManager>>();
            var storageManager = new HddManager(loggerMock.Object, _configuration, mockFileSystem);

            string fileName1 = "mockFile1.txt";
            string fileName2 = "mockFile2.txt";
            string fileName3 = "mockFile3.txt";

            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName2), mockFile2.Contents);
            await mockFileSystem.File.WriteAllBytesAsync(Path.Combine(_basePath, fileName3), mockFile3.Contents);

            Exception ex = await Assert.ThrowsAsync<Exception>(() => storageManager.UpdateFileAsync(fileName1, mockFile1.Contents));
            Assert.Equal("File does not exists on HDD.", ex.Message);
        }

        private MockFileSystem CreateFileSystem()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(_basePath);

            return mockFileSystem;
        }
    }
}