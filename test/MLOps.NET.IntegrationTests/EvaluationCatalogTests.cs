using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class EvaluationCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task LogConfusionMatrixAsync_SavesConfusionMatrixOnRun()
        {
            var runId = await sut.LifeCycle.CreateRunAsync("Test");
            var mlContext = new MLContext(seed: 2);
            List<DataPoint> samples = GetSampleDataForTraining();

            var data = mlContext.Data.LoadFromEnumerable(samples);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.BinaryClassification.Evaluate(predicitions, labelColumnName: "Label");

            //Act
            await sut.Evaluation.LogConfusionMatrixAsync(runId, metrics.ConfusionMatrix);

            //Assert
            var confusionMatrix = sut.Evaluation.GetConfusionMatrix(runId);
            confusionMatrix.Should().NotBeNull();
        }

        private static List<DataPoint> GetSampleDataForTraining()
        {
            return new List<DataPoint>()
            {
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 2} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {1, 0, 0} , Label = true  }
            };
        }

        [TestMethod]
        public async Task LogMetricAsync_ShouldLogMetric()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var id = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Act
            await sut.Evaluation.LogMetricAsync(id, "F1Score", 0.78d);

            //Assert
            var metric = sut.Evaluation.GetMetrics(id).First();
            metric.Should().NotBeNull();
            metric.MetricName.Should().Be("F1Score");
            metric.Value.Should().Be(0.78d);
        }

        [TestMethod]
        public async Task GetConfusionMatrix_NoConfusionMatrixExist_ShouldReturnNull()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Act
            var confusionMatrix = sut.Evaluation.GetConfusionMatrix(runId);

            //Assert
            confusionMatrix.Should().BeNull();
        }
    }

    internal class DataPoint
    {
        [VectorType(3)]
        public float[] Features { get; set; }

        public bool Label { get; set; }
    }
}
