﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using Moq;
using System;
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
        private LifeCycleCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.clockMock = new Mock<IClock>();
            this.experimentRepositoryMock = new Mock<IExperimentRepository>();
            this.runRepositoryMock = new Mock<IRunRepository>();
            this.sut = new LifeCycleCatalog(experimentRepositoryMock.Object, runRepositoryMock.Object, clockMock.Object);
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
            this.runRepositoryMock.Verify(x => x.CreateRunAsync(It.IsAny<Guid>(), gitCommitHash), Times.Once());
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Arrange
            //Act
            var runId = await sut.CreateRunAsync(Guid.NewGuid());

            //Assert
            this.runRepositoryMock.Verify(x => x.CreateRunAsync(It.IsAny<Guid>(), string.Empty), Times.Once());
        }
    }
}
