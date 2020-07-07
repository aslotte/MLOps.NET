using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Entities.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using Moq;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class EvaluationCycleCatalogTests
    {
        private Mock<IMetaDataStore> metaDataStoreMock;
        private EvaluationCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.metaDataStoreMock = new Mock<IMetaDataStore>();
            this.sut = new EvaluationCatalog(metaDataStoreMock.Object);
        }

        [TestMethod]
        public async Task LogMetricsAsync_GivenNaN_ShouldNotLogMetric()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var metric = new { F1Score = double.NaN };

            //Act
            await sut.LogMetricsAsync(runId, metric);

            //Assert
            metaDataStoreMock.Verify(x => x.LogMetricAsync(runId, It.IsAny<string>(), It.IsAny<double>()), Times.Never());
        }

        [TestMethod]
        public async Task LogMetricsAsync_GivenAValidNumber_ShouldLogMetric()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var metric = new { F1Score = 0.56d };

            //Act
            await sut.LogMetricsAsync(runId, metric);

            //Assert
            metaDataStoreMock.Verify(x => x.LogMetricAsync(runId, It.IsAny<string>(), 0.56d), Times.Once());
        }
    }
}
