using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class MetaDataStoreTests
    {
        protected IMLOpsContext sut;

        protected async Task TearDown(IMLOpsDbContext context)
        {
            var experiments = context.Experiments;
            var runs = context.Runs;
            var metrics = context.Metrics;
            var hyperParameters = context.HyperParameters;
            var confusionMatrices = context.ConfusionMatrices;
            var data = context.Data;

            context.Experiments.RemoveRange(experiments);
            context.Runs.RemoveRange(runs);
            context.Metrics.RemoveRange(metrics);
            context.HyperParameters.RemoveRange(hyperParameters);
            context.ConfusionMatrices.RemoveRange(confusionMatrices);
            context.Data.RemoveRange(data);

            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateExperimentAsync_Always_ReturnsNonEmptyGuidAsync()
        {
            //Act
            var guid = await sut.LifeCycle.CreateExperimentAsync("first experiment");

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task CreateRunWithMetrics_GetRunShouldIncludeAssociatedData()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var id = await sut.LifeCycle.CreateRunAsync(experimentId);

            await sut.Evaluation.LogMetricAsync(id, "F1Score", 0.56d);
            await sut.Training.LogHyperParameterAsync(id, "Trainer", "SupportVectorMachine");

            //Act
            var run = sut.LifeCycle.GetRun(id);

            //Assert
            var metric = run.Metrics.First();
            metric.MetricName.Should().Be("F1Score");
            metric.Value.Should().Be(0.56d);

            var hyperParameter = run.HyperParameters.First();
            hyperParameter.ParameterName.Should().Be("Trainer");
            hyperParameter.Value.Should().Be("SupportVectorMachine");
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRun()
        {
            var runId = await sut.LifeCycle.CreateRunAsync("Test");

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act
            await sut.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.TrainingTime.Should().Be(expectedTrainingTime);
        }

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

        [TestMethod]
        public void SetTrainingTimeAsync_NoRunProvided_ThrowsException()
        {
            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act and Assert
            var runId = Guid.NewGuid();
            var expectedMessage = $"The run with id {runId} does not exist";

            Func<Task> func = new Func<Task>(async () => await sut.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime));

            func.Should().Throw<InvalidOperationException>(expectedMessage);
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

        [TestMethod]
        public async Task CreateRunAsync_WithGitCommitHash_SetsGitCommitHash()
        {
            var gitCommitHash = "12323239329392";
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId, gitCommitHash);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(gitCommitHash);
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Act
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(string.Empty);
        }

        [TestMethod]
        public async Task CreateExperimentAsync_Twice_ShouldNotAddDuplicate()
        {
            //Act
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var experimentId2 = await sut.LifeCycle.CreateExperimentAsync("test");

            //Assert
            experimentId.Should().Be(experimentId2);
        }

        [TestMethod]
        public async Task LogDataAsync_GivenValidDataView_ShouldLogData()
        {
            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(runId, data);

            //Assert
            var savedData = sut.Data.GetData(runId);

            savedData.DataSchema.ColumnCount.Should().Be(2);

            savedData.DataSchema.DataColumns
                .Any(x => x.Type == nameof(Boolean) && x.Name == "Sentiment")
                .Should()
                .BeTrue();

            savedData.DataSchema.DataColumns
                .Any(x => x.Type == nameof(String) && x.Name == "Review")
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public async Task GivenARunWithGitCommitHash_ShouldBeAbleToGetRun()
        {
            //Arrange
            var commitHash = "123456789";
            var runId = await sut.LifeCycle.CreateRunAsync("Experiment", commitHash);

            //Act
            var savedRun = sut.LifeCycle.GetRun(commitHash);

            //Assert
            savedRun.RunId.Should().Be(runId);
        }

        private IDataView LoadData()
        {
            var mlContext = new MLContext(seed: 1);

            return mlContext.Data.LoadFromTextFile<ProductReview>("Data/product_reviews.csv", hasHeader: true, separatorChar: ',');
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
    }

    internal class DataPoint
    {
        [VectorType(3)]
        public float[] Features { get; set; }

        public bool Label { get; set; }
    }
}
