using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class ModelCatalogRepositoryTests
    {
        [TestMethod]
        public async Task UploadModelAsync_ValidModelPath_UploadSuccessAsync()
        {
            //Arrange
            var destinationFolder = @"C:\MLOps";
            var modelRepository = new LocalFileModelRepository(new FileSystem(), destinationFolder);
            var runRepositoryMock = new Mock<IRunRepository>();

            var modelCatalog = new ModelCatalog(modelRepository, runRepositoryMock.Object);

            var runId = Guid.NewGuid();
            var modelPath = @"C:\data\model.zip";
            var modelStoragePath = @"C:\MLOps";
            using var writer = new StreamWriter(modelPath);
            writer.Close();

            runRepositoryMock.Setup(x => x.CreateRunArtifact(runId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            //Act
            await modelCatalog.UploadAsync(runId, modelPath);

            //Assert
            var fileExists = File.Exists(Path.Combine(modelStoragePath, $"{runId}.zip"));
            fileExists.Should().BeTrue();
        }
    }
}
