using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Constants;
using MLOps.NET.Tests.Common.Data;
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
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);

            await sut.Evaluation.LogMetricAsync(run.RunId, "F1Score", 0.56d);
            await sut.Training.LogHyperParameterAsync(run.RunId, "Trainer", "SupportVectorMachine");

            //Act
            run = sut.LifeCycle.GetRun(run.RunId);

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
            var run = await sut.LifeCycle.CreateRunAsync("Test");

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act
            await sut.LifeCycle.SetTrainingTimeAsync(run.RunId, expectedTrainingTime);

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);
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
            var run = await sut.LifeCycle.CreateRunAsync(experimentId, gitCommitHash);

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);
            run.GitCommitHash.Should().Be(gitCommitHash);
        }

        [TestMethod]
        public async Task GivenARunWithGitCommitHash_ShouldBeAbleToGetRun()
        {
            //Arrange
            var commitHash = "123456789";
            var run = await sut.LifeCycle.CreateRunAsync("Experiment", commitHash);

            //Act
            var savedRun = sut.LifeCycle.GetRun(commitHash);

            //Assert
            savedRun.RunId.Should().Be(run.RunId);
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Act
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);
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
        public async Task CreateRunAsync_GivenMLNETDependency_ShouldSetPackageDependencies()
        {
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");

            //Act
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);
            run.PackageDepedencies.Count().Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task CreateRunAndExperimentAsync_GivenMLNETDependency_ShouldSetPackageDependencies()
        {
            //Act
            var run = await sut.LifeCycle.CreateRunAsync("test");

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);
            run.PackageDepedencies.Count().Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task RegisterSchema_GivenModelInputAndOutput_ShouldCreateModelSchemas()
        {
            //Arrange
            var run = await sut.LifeCycle.CreateRunAsync("test");

            //Act
            await sut.LifeCycle.RegisterModelSchema<ModelInput, ModelOutput>(run.RunId);

            //Assert
            run = sut.LifeCycle.GetRun(run.RunId);

            run.ModelSchemas.Count().Should().Be(2);
            run.ModelSchemas.FirstOrDefault(x => x.Name == Constant.ModelInput).Should().NotBeNull();
            run.ModelSchemas.FirstOrDefault(x => x.Name == Constant.ModelOutput).Should().NotBeNull();
        }
    }
}
