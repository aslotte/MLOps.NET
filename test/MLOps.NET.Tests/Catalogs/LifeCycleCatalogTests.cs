using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Constants;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services;
using MLOps.NET.Services.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Data;
using MLOps.NET.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class LifeCycleCatalogTests
    {
        private Mock<IClock> clockMock;
        private Mock<IExperimentRepository> experimentRepositoryMock;
        private Mock<IRunRepository> runRepositoryMock;
        private Mock<IPackageDependencyIdentifier> packageDependencyMock;
        private Mock<ISchemaGenerator> schemaGeneratorMock;
        private LifeCycleCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.clockMock = new Mock<IClock>();
            this.experimentRepositoryMock = new Mock<IExperimentRepository>();
            this.runRepositoryMock = new Mock<IRunRepository>();
            this.packageDependencyMock = new Mock<IPackageDependencyIdentifier>();
            this.schemaGeneratorMock = new Mock<ISchemaGenerator>();

            packageDependencyMock.Setup(x => x.IdentifyPackageDependencies()).Returns(new List<PackageDependency>());

            this.sut = new LifeCycleCatalog(experimentRepositoryMock.Object, runRepositoryMock.Object, clockMock.Object, packageDependencyMock.Object, schemaGeneratorMock.Object);
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRunAsEndTimeMinusRunDate()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var runDate = DateTime.UtcNow.AddMinutes(-1);
            var endTime = DateTime.UtcNow;

            var run = new Run(Guid.NewGuid())
            {
                RunDate = runDate
            };

            clockMock.SetupGet(x => x.UtcNow).Returns(endTime);
            
            runRepositoryMock.Setup(x => x.GetRun(runId))
                .Returns(run);

            //Act
            await sut.SetTrainingTimeAsync(runId);

            //Assert
            var expectedTrainingTime = endTime.Subtract(runDate);
            runRepositoryMock.Verify(x => x.SetTrainingTimeAsync(runId, expectedTrainingTime), Times.Once());
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTime_CallsMetaStoreWithTimeSpan()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var runDate = DateTime.UtcNow.AddMinutes(-1);
            var endTime = DateTime.UtcNow;

            var trainingTime = endTime.Subtract(runDate);

            //Act
            await sut.SetTrainingTimeAsync(runId, trainingTime);

            //Assert
            runRepositoryMock.Verify(x => x.SetTrainingTimeAsync(runId, trainingTime), Times.Once());
        }

        [TestMethod]
        public async Task CreateRunAsync_WithGitCommitHash_SetsGitCommitHash()
        {
            //Arrange
            var gitCommitHash = "12323239329392";

            //Act
            var runId = await sut.CreateRunAsync(Guid.NewGuid(), gitCommitHash);

            //Assert
            this.runRepositoryMock.Verify(x => x.CreateRunAsync(It.IsAny<Guid>(), new List<PackageDependency>(), gitCommitHash), Times.Once());
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Arrange
            //Act
            var runId = await sut.CreateRunAsync(Guid.NewGuid());

            //Assert
            this.runRepositoryMock.Verify(x => x.CreateRunAsync(It.IsAny<Guid>(), new List<PackageDependency>(), string.Empty), Times.Once());
        }

        [TestMethod]
        public async Task CreateRunAsync_WithDependencies_ShouldProvidePackageDependencies()
        {
            //Arrange
            var dependencies = new List<PackageDependency>
            {
                new PackageDependency
                {
                    Name = "Microsoft.ML",
                    Version = "1.5.2"
                }
            };

            packageDependencyMock.Setup(x => x.IdentifyPackageDependencies()).Returns(dependencies);

            //Act
            var runId = await sut.CreateRunAsync(Guid.NewGuid());

            //Assert
            this.runRepositoryMock.Verify(x => x.CreateRunAsync(It.IsAny<Guid>(), dependencies, string.Empty), Times.Once());
        }

        [TestMethod]
        public async Task RegisterSchema_ShouldCallSchemaGenerator()
        {
            //Arrange

            //Act
            await sut.RegisterModelSchema<ModelInput, ModelOutput>(Guid.NewGuid());

            //Assert
            this.schemaGeneratorMock.Verify(x => x.GenerateDefinition<ModelInput>(Constant.ModelInput), Times.Once());
            this.schemaGeneratorMock.Verify(x => x.GenerateDefinition<ModelOutput>(Constant.ModelOutput), Times.Once());
        }

        [TestMethod]
        public async Task RegisterSchema_ShouldCallRunRepoitory()
        {
            //Arrange
            var runId = Guid.NewGuid();

            //Act
            await sut.RegisterModelSchema<ModelInput, ModelOutput>(runId);

            //Assert
            this.runRepositoryMock.Verify(x => x.CreateModelSchemaAsync(runId, It.IsAny<List<ModelSchema>>()), Times.Once());
        }
    }
}
