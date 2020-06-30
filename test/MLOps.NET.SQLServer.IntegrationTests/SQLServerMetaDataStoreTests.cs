using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities;
using MLOps.NET.SQLServer.IntegrationTests.Data;
using MLOps.NET.SQLServer.Storage;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    [TestClass]
    [TestCategory("IntegrationTestSqlServer")]
    public class SQLServerMetaDataStoreTests
    {
        private const string connectionString = "Server=localhost,1433;Database=MLOpsNET_IntegrationTests;User Id=sa;Password=MLOps4TheWin!;";
        private SQLServerMetaDataStore sut;

        [TestInitialize]
        public void Initialize()
        {
            sut = new SQLServerMetaDataStore(new DbContextFactory(connectionString));
        }

        [TestCleanup]
        public async Task TearDown()
        {
            var contextFactory = new DbContextFactory(connectionString);
            var context = contextFactory.CreateDbContext();

            var experiments = context.Experiments;
            var runs = context.Runs;
            var metrics = context.Metrics;
            var hyperParameters = context.HyperParameters;
            var confusionMatrices = context.ConfusionMatrices;
            var data = context.Data;
            var dataSchema = context.DataSchemas;
            var dataColumns = context.DataColumns;

            context.Experiments.RemoveRange(experiments);
            context.Runs.RemoveRange(runs);
            context.Metrics.RemoveRange(metrics);
            context.HyperParameters.RemoveRange(hyperParameters);
            context.Data.RemoveRange(data);
            context.DataSchemas.RemoveRange(dataSchema);
            context.DataColumns.RemoveRange(dataColumns);

            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateExperimentAsync_ShouldCreateAnExperiment()
        {
            //Act
            var id = await sut.CreateExperimentAsync("test");

            //Assert
            var experiement = sut.GetExperiment("test");
            experiement.Should().NotBeNull();
            experiement.Id.Should().Be(id);
        }

        [TestMethod]
        public async Task CreateExperimentAsync_Twice_ShouldNotAddDuplicate()
        {
            //Act
            var experimentId = await sut.CreateExperimentAsync("test");
            var experimentId2 = await sut.CreateExperimentAsync("test");

            //Assert
            experimentId.Should().Be(experimentId2);
        }

        [TestMethod]
        public async Task CreateRunAsync_ShouldCreateRun()
        {
            //Act
            var experimentId = await sut.CreateExperimentAsync("test");
            var id = await sut.CreateRunAsync(experimentId);

            //Assert
            var run = sut.GetRun(id);
            run.Should().NotBeNull();
            run.Id.Should().Be(id);
        }

        [TestMethod]
        public async Task LogMetricAsync_ShouldLogMetric()
        {
            //Arrange
            var experimentId = await sut.CreateExperimentAsync("test");
            var id = await sut.CreateRunAsync(experimentId);

            //Act
            await sut.LogMetricAsync(id, "F1Score", 0.78d);

            //Assert
            var metric = sut.GetMetrics(id).First();
            metric.Should().NotBeNull();
            metric.MetricName.Should().Be("F1Score");
            metric.Value.Should().Be(0.78d);
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_ShouldTrainingTime()
        {
            //Arrange
            var experimentId = await sut.CreateExperimentAsync("test");
            var id = await sut.CreateRunAsync(experimentId);

            var trainingTime = new System.TimeSpan(0, 5, 0);

            //Act
            await sut.SetTrainingTimeAsync(id, trainingTime);

            //Assert
            var run = sut.GetRun(id);
            run.TrainingTime.Should().Be(trainingTime);
        }

        [TestMethod]
        public async Task LogConfusionMatrixAsync_SavesConfusionMatrixOnRun()
        {
            //Arrange
            var experimentId = await sut.CreateExperimentAsync("test");
            var runId = await sut.CreateRunAsync(experimentId);

            var expectedConfusionMatrix = new ConfusionMatrix
            {
                PerClassPrecision = new List<double> { 0.99d, 0.44d },
                PerClassRecall = new List<double> { 0.77d, 0.88d },
                Counts = new List<List<double>>
                {
                    new List<double> { 9, 1 },
                    new List<double> { 4, 33}
                },
                NumberOfClasses = 2
            };

            //Act
            await sut.LogConfusionMatrixAsync(runId, expectedConfusionMatrix);

            //Assert
            var confusionMatrix = sut.GetConfusionMatrix(runId);

            confusionMatrix.Should().NotBeNull();
            confusionMatrix.Should().BeEquivalentTo(expectedConfusionMatrix);
        }

        [TestMethod]
        public async Task GetConfusionMatrix_NoConfusionMatrixExist_ShouldReturnNull()
        {
            //Arrange
            var experimentId = await sut.CreateExperimentAsync("test");
            var runId = await sut.CreateRunAsync(experimentId);

            //Act
            var confusionMatrix = sut.GetConfusionMatrix(runId);

            //Assert
            confusionMatrix.Should().BeNull();
        }

        [TestMethod]
        public async Task LogDataAsync_GivenValidDatView_ShouldLogData()
        {
            //Arrange
            var experimentId = await sut.CreateExperimentAsync("test");
            var runId = await sut.CreateRunAsync(experimentId);

            var data = LoadData();

            //Act
            await sut.LogDataAsync(runId, data);

            //Assert
            var savedData = sut.GetData(runId);

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
    }
}
