using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests.ModelRepository
{
    [TestClass]
    public class LocalFileModelRepositoryTests
    {
        private LocalFileModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var destinationFolder = @"C:\mlops";
            this.sut = new LocalFileModelRepository(new FileSystem(), new ModelPathGenerator(), destinationFolder);
        }

        [TestMethod]
        public async Task DeployModel_GivenAnExistingModel_ShouldDeployModelToDeploymentTargetFolder()
        {
            //Arrange
            var experiment = new Experiment("ExperimentName");

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                ExperimentId = experiment.ExperimentId,
            };

            var deploymentTarget = new DeploymentTarget("Test");

            await sut.UploadModelAsync(registeredModel.RunId, @"Data/model.txt");

            //Act
            var deployedPath = await sut.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            //Assert
            File.Exists(deployedPath).Should().BeTrue();
        }

        [TestMethod]
        public async Task DeployModel_GivenAnAlreadyDeployedModel_ShouldOverwriteTheExistingModel()
        {
            //Arrange
            var experiment = new Experiment("ExperimentName");

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                ExperimentId = experiment.ExperimentId,
            };

            var registeredModel1 = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                ExperimentId = experiment.ExperimentId,
            };

            var deploymentTarget = new DeploymentTarget("Test");

            await sut.UploadModelAsync(registeredModel.RunId, @"Data/model.txt");
            await sut.UploadModelAsync(registeredModel1.RunId, @"Data/model.txt");

            var deployedPath = await sut.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            var fileInfo = new FileInfo(deployedPath);
            var lastUpdateTime = fileInfo.LastAccessTime;

            //Act
            deployedPath = await sut.DeployModelAsync(deploymentTarget, registeredModel1, experiment);

            //Assert
            fileInfo.Refresh();
            var lastUpdateTimeNewModel = fileInfo.LastAccessTime;
            
            File.Exists(deployedPath).Should().BeTrue();
            lastUpdateTime.Ticks.Should().BeLessThan(lastUpdateTimeNewModel.Ticks);
        }

        [TestMethod]
        public async Task GetDeploymentUri_GivenADeployedModel_ShouldReturnAValidUri()
        {
            //Arrange
            var runId = Guid.NewGuid();
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            var experiment = new Experiment("ExperimentName");

            var registeredModel = new RegisteredModel
            {
                RunId = runId
            };

            var deploymentTarget = new DeploymentTarget("Test");
            await sut.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            //Act
            var deployedPath = sut.GetDeploymentUri(experiment, deploymentTarget);

            //Assert
            File.Exists(deployedPath).Should().BeTrue();
        }
    }
}
