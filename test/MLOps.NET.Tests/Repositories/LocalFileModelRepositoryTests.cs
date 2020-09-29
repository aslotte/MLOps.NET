using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using System;
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
        private MockFileSystem mockFileSystem;
        private LocalFileModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockFileSystem = new MockFileSystem();
            sut = new LocalFileModelRepository(mockFileSystem, new ModelPathGenerator());
        }

        [TestMethod]
        public async Task UploadModel_ShouldCreateFolderIfNotExists()
        {
            // Arrange
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "model-repository");
            mockFileSystem.AddFile("model.zip", new MockFileData("test"));

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockFileSystem.Directory.Exists(folderPath).Should().Be(true);
        }

        [TestMethod]
        public async Task UploadModel_ShouldSaveFile()
        {
            // Arrange
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");

            mockFileSystem.AddFile("model.zip", new MockFileData("test"));
            var expectedFilePath = mockFileSystem.Path.Combine(folderPath, "model-repository", $"{new Guid()}.zip");

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
            var filePath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "model-repository", $"{runId}.zip");
            mockFileSystem.AddFile(filePath, new MockFileData("test"));
            using var memStream = new MemoryStream();

            // Act
            await sut.DownloadModelAsync(runId, memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            // Assert
            Encoding.Default.GetString(memStream.ToArray()).Should().Be("test");
        }


        [TestMethod]
        public async Task UploadModelAsync_ShouldSetPositionToZero()
        {
            //Arrange       
            var runId = Guid.NewGuid();

            //Act
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            //Assert
            using var memoryStream = new MemoryStream();
            await sut.DownloadModelAsync(runId, memoryStream);

            memoryStream.Should().NotBeNull();
            memoryStream.Position.Should().Be(0);
        }

        [TestMethod]
        public async Task DownloadModel_ThrowsIfFileDoesNotExist()
        {
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "model-repository");
            using var memStream = new MemoryStream();

            // Act
            var downloadAction = new Func<Task>(() => sut.DownloadModelAsync(new Guid(), memStream));

            // Assert
            await downloadAction.Should().ThrowExactlyAsync<FileNotFoundException>("Because provided file does not exist on disk");
        }

        [TestMethod]
        public async Task DeployModel_ShouldReturnCorrectDeploymentPath()
        {
            var folderPath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            var experiment = new Experiment("ExperimentName");

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
            };

            var deploymentTarget = new DeploymentTarget("Test");

            var expectedPath = Path.Combine(folderPath, "deployment", "ExperimentName", "Test", $"{"ExperimentName"}.zip");
            var sourcePath = Path.Combine(folderPath, "model-repository", $"{registeredModel.RunId}.zip");
            mockFileSystem.AddFile(sourcePath, new MockFileData("test"));

            // Act
            var deployedPath = await sut.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            // Assert
            deployedPath.Should().Be(expectedPath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The model to be deployed does not exist")]
        public async Task DeployModel_NoSourceFileExists_ShouldThrowException()
        {
            var experiment = new Experiment("ExperimentName");

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
            };

            var deploymentTarget = new DeploymentTarget("Test");

            // Act
            await sut.DeployModelAsync(deploymentTarget, registeredModel, experiment);
        }
    }
}
