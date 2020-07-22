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
        private Mock<IModelLabelRepository> modelLabelRepositoryMock;
        private EvaluationCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.metricRepositoryMock = new Mock<IMetricRepository>();
            this.confusionMatrixRepositoryMock = new Mock<IConfusionMatrixRepository>();
            this.modelLabelRepositoryMock = new Mock<IModelLabelRepository>();
            this.sut = new EvaluationCatalog(metricRepositoryMock.Object, confusionMatrixRepositoryMock.Object, modelLabelRepositoryMock.Object);
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

        [TestMethod]
        public async Task LogModelLabelAsync_GivenValidStrings_ShouldLogModelLabel()
        {
            //Arrange
            var runArtifactId = Guid.NewGuid();
            var modelLabel = "Department";
            var modelLabelValue = "Engineering";

            //Act
            await sut.LogModelLabelAsync(runArtifactId, modelLabel, modelLabelValue);

            //Assert
            modelLabelRepositoryMock.Verify(x => x.LogModelLabelAsync(runArtifactId, modelLabel, modelLabelValue), Times.Once());
        }
    }
}
