using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class LocalFileModelRepositoryTests
    {
        [TestMethod]
        public async Task UploadModel_ShouldCreateFolderIfNotExists()
        {   
            // Arrange
            var mockFileSystem = new MockFileSystem();
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            mockFileSystem.AddFile("model.zip", new MockFileData("test"));
            var sut = new LocalFileModelRepository(mockFileSystem);

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockFileSystem.Directory.Exists(folderPath).Should().Be(true);
        }

        [TestMethod]
        public async Task UploadModel_ShouldSaveFile()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            mockFileSystem.AddFile("model.zip", new MockFileData("test"));
            var sut = new LocalFileModelRepository(mockFileSystem);
            var expectedFilePath = mockFileSystem.Path.Combine(folderPath, $"{new Guid()}.zip");

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockFileSystem.FileExists(expectedFilePath).Should().Be(true);
        }

        [TestMethod]
        public async Task DownloadModel_ShouldReturnFileDataFromDisk()
        {
            // Arrange
            var runId = Guid.NewGuid();
            var mockFileSystem = new MockFileSystem();
            var filePath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", $"{runId}.zip");
            mockFileSystem.AddFile(filePath, new MockFileData("test"));
            var sut = new LocalFileModelRepository(mockFileSystem);
            using var memStream = new MemoryStream();

            // Act
            await sut.DownloadModelAsync(runId, memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            // Assert
            Encoding.Default.GetString(memStream.ToArray()).Should().Be("test");
        }

        [TestMethod]
        public async Task DownloadModel_ThrowsIfFileDoesNotExist()
        {
            var mockFileSystem = new MockFileSystem();
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            var sut = new LocalFileModelRepository(mockFileSystem);
            using var memStream = new MemoryStream();

            // Act
            var downloadAction = new Func<Task>(() => sut.DownloadModelAsync(new Guid(), memStream));

            // Assert
            await downloadAction.Should().ThrowExactlyAsync<FileNotFoundException>("Because provided file does not exist on disk");
        }
    }
}
