using System;
using System.Collections.Generic;
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


        [TestMethod]
        public async Task MLOpsContext_ShouldSaveConfusionMatrixIfPassedABinaryClassifier()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            List<DataPoint> samples = GetSampleDataForTraining();

            var data = mlContext.Data.LoadFromEnumerable(samples);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.BinaryClassification.Evaluate(predicitions, labelColumnName: "Label");

            mockMetaDataStore.Setup(s => s.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.Evaluation.LogMetricsAsync<CalibratedBinaryClassificationMetrics>(Guid.NewGuid(), metrics);

            // Assert
            mockMetaDataStore.Verify(c => c.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task MLOpsContext_ShouldNotSaveConfusionMatrixIfPassedNotABinaryClassifier()
        {
            // Arrange
            var mlContext = new MLContext(seed: 2);
            List<DataPoint> samples = GetSampleDataForTraining();

            var data = mlContext.Data.LoadFromEnumerable(samples);
            var options = new RandomizedPcaTrainer.Options()
            {
                FeatureColumnName = nameof(DataPoint.Features),
                Rank = 1,
                EnsureZeroMean = false,
                Seed = 10
            };
            var trainer = mlContext.AnomalyDetection.Trainers.RandomizedPca(options).
                Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: nameof(DataPoint.Label), inputColumnName: nameof(DataPoint.Label)));

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.AnomalyDetection.Evaluate(predicitions);

            mockMetaDataStore.Setup(s => s.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.Evaluation.LogMetricsAsync<AnomalyDetectionMetrics>(Guid.NewGuid(), metrics);

            // Assert
            mockMetaDataStore.Verify(c => c.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        private static List<DataPoint> GetSampleDataForTraining()
        {
            return new List<DataPoint>()
            {
                new DataPoint(){ Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint(){ Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint(){ Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint(){ Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint(){ Features = new float[3] {0, 2, 2} , Label = false },
                new DataPoint(){ Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint(){ Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint(){ Features = new float[3] {1, 0, 0} , Label = true  }
            };
        }
    }

    internal class NotTrainer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    internal class DataPoint
    {
        public DataPoint()
        {
        }

        [VectorType(3)]
        public float[] Features { get; set; }

        public bool Label { get; set; }
    }
}
