using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using Moq;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class LifeCycleCatalogTests
    {
        private Mock<IClock> clockMock;
        private Mock<IMetaDataStore> metaDataStoreMock;
        private LifeCycleCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.clockMock = new Mock<IClock>();
            this.metaDataStoreMock = new Mock<IMetaDataStore>();
            this.sut = new LifeCycleCatalog(metaDataStoreMock.Object, clockMock.Object);
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRunAsEndTimeMinusRunDate()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var runDate = DateTime.UtcNow.AddMinutes(-1);
            var endTime = DateTime.UtcNow;

            var runMock = new Mock<IRun>();
            runMock.SetupGet(x => x.RunDate).Returns(runDate);
            clockMock.SetupGet(x => x.UtcNow).Returns(endTime);
            
            metaDataStoreMock.Setup(x => x.GetRun(runId))
                .Returns(runMock.Object);

            //Act
            await sut.SetTrainingTimeAsync(runId);

            //Assert
            var expectedTrainingTime = endTime.Subtract(runDate);
            metaDataStoreMock.Verify(x => x.SetTrainingTimeAsync(runId, expectedTrainingTime), Times.Once());
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
            metaDataStoreMock.Verify(x => x.SetTrainingTimeAsync(runId, trainingTime), Times.Once());
        }
    }
}
