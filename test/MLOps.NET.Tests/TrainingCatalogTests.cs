using Microsoft.ML;
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
    public class TrainingCatalogTests
    {
        private readonly TrainingCatalog sut;
        private readonly Mock<IHyperParameterRepository> hyperParameterRepositoryMock;

        public TrainingCatalogTests()
        {
            this.hyperParameterRepositoryMock = new Mock<IHyperParameterRepository>();

            sut = new TrainingCatalog(hyperParameterRepositoryMock.Object);
        }

        [TestMethod]
        public async Task MLOpsContext_ShouldCallLogHyperParameterIfPassedATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Sentiment", featureColumnName: "Features");
            hyperParameterRepositoryMock.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.LogHyperParametersAsync(new Guid(), trainer);

            // Assert
            hyperParameterRepositoryMock.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }


        [TestMethod]
        public async Task MLOpsContext_ShouldNotCallLogHyperParameterIfPassedNotATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            hyperParameterRepositoryMock.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));
            var notTrainer = new NotTrainer();

            // Act
            await sut.LogHyperParametersAsync(new Guid(), notTrainer);

            // Assert
            hyperParameterRepositoryMock.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

    internal class NotTrainer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
