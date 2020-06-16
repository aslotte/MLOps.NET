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
    public class EvaluationCatalogTests
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

            metaDataStoreMock.Setup(s => s.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.LogConfusionMatrixAsync<CalibratedBinaryClassificationMetrics>(Guid.NewGuid(), metrics);

            // Assert
            metaDataStoreMock.Verify(c => c.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.AtLeastOnce);
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

            metaDataStoreMock.Setup(s => s.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(""));

            // Act
            await sut.LogConfusionMatrixAsync<AnomalyDetectionMetrics>(Guid.NewGuid(), metrics);

            // Assert
            metaDataStoreMock.Verify(c => c.LogConfusionMatrixAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
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
