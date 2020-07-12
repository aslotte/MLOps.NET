using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class LifeCycleCatalogTests : RepositoryTests
    {

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
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId, gitCommitHash);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(gitCommitHash);
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
    }
}
