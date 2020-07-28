using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using Moq;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class EvaluationCatalogTests
    {
        private Mock<IMetricRepository> metricRepositoryMock;
        private Mock<IConfusionMatrixRepository> confusionMatrixRepositoryMock;
        private EvaluationCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.metricRepositoryMock = new Mock<IMetricRepository>();
            this.confusionMatrixRepositoryMock = new Mock<IConfusionMatrixRepository>();
            this.sut = new EvaluationCatalog(metricRepositoryMock.Object, confusionMatrixRepositoryMock.Object);
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
            metricRepositoryMock.Verify(x => x.LogMetricAsync(runId, It.IsAny<string>(), It.IsAny<double>()), Times.Never());
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
            metricRepositoryMock.Verify(x => x.LogMetricAsync(runId, It.IsAny<string>(), 0.56d), Times.Once());
        }
    }
}
