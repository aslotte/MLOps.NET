using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.Tests
{
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
            await Task.Run(() => sut.UploadModelAsync(new Guid(), "model.zip"));

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
            await Task.Run(() => sut.UploadModelAsync(new Guid(), "model.zip"));

            // Assert
            mockFileSystem.FileExists(expectedFilePath).Should().Be(true);
        }
    }
}
