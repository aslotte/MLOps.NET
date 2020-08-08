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
        private ModelCatalog sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var destinationFolder = @"C:\MLOps";
            var modelRepository = new LocalFileModelRepository(new FileSystem(), destinationFolder);
            var runRepositoryMock = new Mock<IRunRepository>();

            runRepositoryMock
                .Setup(x => x.CreateRunArtifact(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            this.sut = new ModelCatalog(modelRepository, runRepositoryMock.Object);
        }

        [TestMethod]
        public async Task UploadModelAsync_ValidModelPath_UploadSuccessAsync()
        {
            //Arrange
            var runId = Guid.NewGuid();

            var modelRepositoryPath = @"C:\MLOps\model-repository";
            var expectedModelPath = Path.Combine(modelRepositoryPath, $"{runId}.zip");

            //Act
            await sut.UploadAsync(runId, @"Data/model.txt");

            //Assert
            File.Exists(expectedModelPath).Should().BeTrue();
        }
    }
}