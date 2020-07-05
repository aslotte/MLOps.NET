using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class SQLiteMetaDataStoreTests
    {
        [TestMethod]
        public async Task CreateExperimentAsync_Always_ReturnsNonEmptyGuidAsync()
        {
            //Arrange
            IMLOpsContext sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLite()
                .Build();

            //Act
            var guid = await sut.LifeCycle.CreateExperimentAsync("first experiment");

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRun()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();
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
            //Arrange
            var unitUnderTest = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();
            var runId = await unitUnderTest.LifeCycle.CreateRunAsync("Test");
            var mlContext = new MLContext(seed: 2);
            List<DataPoint> samples = GetSampleDataForTraining();

            var data = mlContext.Data.LoadFromEnumerable(samples);
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.BinaryClassification.Evaluate(predicitions, labelColumnName: "Label");

            //Act
            await unitUnderTest.Evaluation.LogConfusionMatrixAsync(runId, metrics.ConfusionMatrix);

            //Assert
            var confusionMatrix = unitUnderTest.Evaluation.GetConfusionMatrix(runId);
            confusionMatrix.Should().NotBeNull();
        }

            [TestMethod]
        public void SetTrainingTimeAsync_NoRunProvided_ThrowsException()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

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
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

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
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(Guid.NewGuid());

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(string.Empty);
        }

        [TestMethod]
        public async Task LogDataAsync_GivenValidDatView_ShouldLogData()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(runId, data);

            //Assert
            var savedData = sut.Data.GetData(runId);

            savedData.DataSchema.ColumnCount.Should().Be(2);

            savedData.DataSchema.DataColumns[0].Type.Should().Be(nameof(Boolean));
            savedData.DataSchema.DataColumns[0].Name.Should().Be("Sentiment");

            savedData.DataSchema.DataColumns[1].Type.Should().Be(nameof(String));
            savedData.DataSchema.DataColumns[1].Name.Should().Be("Review");
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
