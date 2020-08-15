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
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Act
            await sut.Model.UploadAsync(run.RunId, "");

            //Assert
            var runArtifacts = sut.Model.GetRunArtifacts(run.RunId);
            runArtifacts.First().RunId.Should().Be(run.RunId);
            runArtifacts.First().Name.Should().Be($"{run.RunId}.zip");
        }

        [TestMethod]
        public async Task RegisterModel_ShouldReturnPopulatedRegisterModel()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(run.RunId, "");

            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();

            //Act
            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");

            //Assert
            registeredModel.Version.Should().Be(1);
            registeredModel.RegisteredDate.Date.Should().Be(DateTime.UtcNow.Date);
            registeredModel.RegisteredBy.Should().Be("The MLOps.NET Team");
            registeredModel.RunArtifactId.Should().Be(runArtifact.RunArtifactId);
        }

        [TestMethod]
        public async Task RegisterModel_GivenNoDescription_ShouldRegisterModelWithoutDesription()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(run.RunId, "");

            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();

            //Act
            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");

            //Assert
            registeredModel.Description.Should().Be(string.Empty);
        }

        [TestMethod]
        public async Task RegisterModel_GivenAModelIsAlreadyRegistered_ShouldRegisterModelWithHigherVersion()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");

            var run = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(run.RunId, "");
            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();
            await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");

            var run2 = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(run2.RunId, "");
            var runArtifact2 = sut.Model.GetRunArtifacts(run2.RunId).First();

            //Act
            await sut.Model.RegisterModel(experimentId, runArtifact2.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");

            //Assert
            var registeredModel = sut.Model.GetLatestRegisteredModel(experimentId);
            registeredModel.Version.Should().Be(2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The RunArtifact with id 00000000-0000-0000-0000-000000000000 does not exist. Unable to register a model")]
        public async Task RegisterModel_GivenRunArtifactDoesNotExist_ThrowsException()
        {
            //Act
            await sut.Model.RegisterModel(Guid.NewGuid(), Guid.Empty, "The MLOps.NET Team", "Model Registered By Test");
        }
    }
}
