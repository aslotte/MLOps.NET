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
            var deploymentTarget = await CreateDeploymentTarget("Prod");

            //Act
            await sut.Deployment.DeployModelAsync(deploymentTarget, registeredModel, "By me");

            //Assert
            var deployment = sut.Deployment.GetDeployments(registeredModel.ExperimentId).First();

            deployment.DeployedBy.Should().Be("By me");
            deployment.DeploymentDate.Date.Should().Be(DateTime.UtcNow.Date);
        }

        [TestMethod]
        public async Task GetRegisteredModel_GivenARegisteredModelHasMoreThanOneDeployment_ShouldPopulateDeployments()
        {
            //Arrange
            var registeredModel = await CreateRegisteredModel();
            var testDeploymentTarget = await CreateDeploymentTarget("Test");
            var prodDeploymentTarget = await CreateDeploymentTarget("Prod");

            await sut.Deployment.DeployModelAsync(testDeploymentTarget, registeredModel, "By me");
            await sut.Deployment.DeployModelAsync(prodDeploymentTarget, registeredModel, "By me");

            //Act
            registeredModel = sut.Model.GetLatestRegisteredModel(registeredModel.ExperimentId);

            //Assert
            registeredModel.Deployments.Should().NotBeNull();
            registeredModel.Deployments.Should().HaveCount(2);
        }

        private async Task<RegisteredModel> CreateRegisteredModel()
        {
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);
            await sut.Model.UploadAsync(run.RunId, "");

            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();
            await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");

            return sut.Model.GetLatestRegisteredModel(experimentId);
        }

        private async Task<DeploymentTarget> CreateDeploymentTarget(string deploymentTargetName)
        {
            await sut.Deployment.CreateDeploymentTargetAsync(deploymentTargetName);

            return sut.Deployment.GetDeploymentTargets().First();
        }
    }
}
