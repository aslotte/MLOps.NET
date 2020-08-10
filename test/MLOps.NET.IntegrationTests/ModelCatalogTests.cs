using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    [TestClass]
    public class ModelCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task UploadModelAsync_ShouldCreateRunArtifact()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Act
            await sut.Model.UploadAsync(runId, "");

            //Assert
            var runArtifacts = sut.Model.GetRunArtifacts(runId);
            runArtifacts.First().RunId.Should().Be(runId);
            runArtifacts.First().Name.Should().Be($"{runId}.zip");
        }

        [TestMethod]
        public async Task GetRunArtifacts_ShouldReturnPopulatedRunArtifacts()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId, "");

            //Act
            var runArtifact = sut.Model.GetRunArtifacts(runId).First();

            //Assert
            runArtifact.Run.Should().NotBeNull();
        }

        [TestMethod]
        public async Task RegisterModel_ShouldReturnPopulatedRegisterModel()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();

            //Act
            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            //Assert
            registeredModel.Version.Should().Be(1);
            registeredModel.RegisteredDate.Date.Should().Be(DateTime.UtcNow.Date);
            registeredModel.RegisteredBy.Should().Be("The MLOps.NET Team");
            registeredModel.RunArtifactId.Should().Be(runArtifact.RunArtifactId);
            registeredModel.Run.Should().NotBeNull();
            registeredModel.Experiment.Should().NotBeNull();
        }

        [TestMethod]
        public async Task RegisterModel_GivenARunWithMetrics_ShouldReturnPopulatedRegisterModel()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            await sut.Evaluation.LogMetricAsync(runId, "Metric", 0.44d);
            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();

            //Act
            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            //Assert
            registeredModel.Run.Metrics.First().MetricName.Should().Be("Metric");
        }

        [TestMethod]
        public async Task RegisterModel_GivenARunWithHyperParameters_ShouldReturnPopulatedRegisterModel()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            await sut.Training.LogHyperParameterAsync(runId, "Parameter", "Value");
            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();

            //Act
            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            //Assert
            registeredModel.Run.HyperParameters.First().ParameterName.Should().Be("Parameter");
        }

        [TestMethod]
        public async Task GetRegisteredModels_ShouldReturnWithRunAndExperimentsLoaded()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();
            await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            //Act
            var registeredModel = sut.Model.GetRegisteredModels(experimentId).First();

            //Assert
            registeredModel.RunArtifact.Should().NotBeNull();
            registeredModel.RunArtifact.Run.Should().NotBeNull();
            registeredModel.RunArtifact.Run.Experiment.Should().NotBeNull();
        }

        [TestMethod]
        public async Task RegisterModel_GivenAModelIsAlreadyRegistered_ShouldRegisterModelWithHigherVersion()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");

            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId, "");
            var runArtifact = sut.Model.GetRunArtifacts(runId).First();
            await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            var runId2 = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId2, "");
            var runArtifact2 = sut.Model.GetRunArtifacts(runId2).First();

            //Act
            await sut.Model.RegisterModel(experimentId, runArtifact2.RunArtifactId, "The MLOps.NET Team");

            //Assert
            var registeredModel = sut.Model.GetLatestRegisteredModel(experimentId);
            registeredModel.Version.Should().Be(2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The RunArtifact with id 00000000-0000-0000-0000-000000000000 does not exist. Unable to register a model")]
        public async Task RegisterModel_GivenRunArtifactDoesNotExist_ThrowsException()
        {
            //Act
            await sut.Model.RegisterModel(Guid.NewGuid(), Guid.Empty, "The MLOps.NET Team");
        }
    }
}
