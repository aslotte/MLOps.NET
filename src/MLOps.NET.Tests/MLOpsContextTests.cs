using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class MLOpsContextTests
    {
        private readonly IMLOpsContext sut;
        private readonly Mock<IMetaDataStore> mockMetaDataStore = new Mock<IMetaDataStore>();
        private readonly Mock<IModelRepository> mockModelRepository = new Mock<IModelRepository>();

        public MLOpsContextTests()
        {
            sut = new MLOpsBuilder()
                .UseMetaDataStore(mockMetaDataStore.Object)
                .UseModelRepository(mockModelRepository.Object)
                .Build();
        }


        [TestMethod]
        public async Task MLOpsContext_ShouldCallLogHyperParameterIfPassedATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Sentiment", featureColumnName: "Features");
            mockMetaDataStore.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.Training.LogHyperParametersAsync<LbfgsLogisticRegressionBinaryTrainer>(new Guid(), trainer);

            // Assert
            mockMetaDataStore.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        
        [TestMethod]
        public async Task MLOpsContext_ShouldNotCallLogHyperParameterIfPassedNotATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            mockMetaDataStore.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));
            var notTrainer = new NotTrainer();

            // Act
            await sut.Training.LogHyperParametersAsync<NotTrainer>(new Guid(), notTrainer);

            // Assert
            mockMetaDataStore.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

    internal class NotTrainer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
