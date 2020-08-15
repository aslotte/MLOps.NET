using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class DeploymentCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task CreateDeploymentTarget_GivenName_CreatesAValidDeploymentTarget()
        {
            //Act
            await sut.Deployment.CreateDeploymentTargetAsync("Production");

            //Assert
            var deploymentTargets = sut.Deployment.GetDeploymentTargets();
            deploymentTargets.First().Name.Should().Be("Production");
            deploymentTargets.First().CreatedDate.Date.Should().Be(DateTime.UtcNow.Date);
            deploymentTargets.First().IsProduction.Should().BeFalse();
        }

        [TestMethod]
        public async Task CreateDeploymentTarget_GivenNameAndIsProductionIsTrue_CreatesAValidDeploymentTarget()
        {
            //Act
            await sut.Deployment.CreateDeploymentTargetAsync("Production", true);

            //Assert
            var deploymentTargets = sut.Deployment.GetDeploymentTargets();
            deploymentTargets.First().Name.Should().Be("Production");
            deploymentTargets.First().CreatedDate.Date.Should().Be(DateTime.UtcNow.Date);
            deploymentTargets.First().IsProduction.Should().BeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Deployment target name was not specified")]
        public async Task CreateDeploymentTarget_GivenNameIsNullOrEmpty_ThrowsExceptionWithMessage()
        {
            //Act
            await sut.Deployment.CreateDeploymentTargetAsync(null);
        }

        [TestMethod]
        public async Task CreateDeployment_ShouldCreateDeployment()
        {
            //Arrange
            var registeredModel = await CreateRegisteredModel();
            var deploymentTarget = await CreateDeploymentTarget();

            //Act
            await sut.Deployment.DeployModelAsync(deploymentTarget, registeredModel, "By me");

            //Assert
            var deployment = sut.Deployment.GetDeployments(registeredModel.ExperimentId).First();

            deployment.RegisteredModel.Should().NotBeNull();
            deployment.RegisteredModelId.Should().Be(registeredModel.RegisteredModelId);
            deployment.DeployedBy.Should().Be("By me");
            deployment.DeploymentDate.Date.Should().Be(DateTime.UtcNow.Date);
            deployment.Experiment.Should().NotBeNull();
        }

        private async Task<RegisteredModel> CreateRegisteredModel()
        {
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();
            await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");

            return sut.Model.GetLatestRegisteredModel(experimentId);
        }

        private async Task<DeploymentTarget> CreateDeploymentTarget()
        {
            await sut.Deployment.CreateDeploymentTargetAsync("Production");

            return sut.Deployment.GetDeploymentTargets().First();
        }
    }
}
