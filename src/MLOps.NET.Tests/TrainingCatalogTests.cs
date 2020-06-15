using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class TrainingCatalogTests
    {
        private Mock<IMetaDataStore> metaDataStoreMock;
        private TrainingCatalog sut;

        [TestInitialize]
        public void Initialize()
        {
            this.metaDataStoreMock = new Mock<IMetaDataStore>();
            this.sut = new TrainingCatalog(metaDataStoreMock.Object);
        }

        [TestMethod]
        public async Task MLOpsContext_ShouldCallLogHyperParameterIfPassedATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Sentiment", featureColumnName: "Features");
            metaDataStoreMock.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.LogHyperParametersAsync<LbfgsLogisticRegressionBinaryTrainer>(new Guid(), trainer);

            // Assert
            metaDataStoreMock.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }


        [TestMethod]
        public async Task MLOpsContext_ShouldNotCallLogHyperParameterIfPassedNotATrainerObject()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            metaDataStoreMock.Setup(s => s.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(""));
            var notTrainer = new NotTrainer();

            // Act
            await sut.LogHyperParametersAsync<NotTrainer>(new Guid(), notTrainer);
            // Assert
            metaDataStoreMock.Verify(c => c.LogHyperParameterAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
