using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class MetaDataStoreTests
    {
        private IMLOpsContext sut;

        [TestInitialize]
        public void Initialize()
        {
            sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLite()
                .Build();
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
        public async Task CreateRunAsync_WithGitCommitHash_SetsGitCommitHash()
        {
            var gitCommitHash = "12323239329392";

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(Guid.NewGuid(), gitCommitHash);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(gitCommitHash);
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(Guid.NewGuid());

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
