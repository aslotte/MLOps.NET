using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests.ModelRepository
{
    [TestClass]
    public class LocalFileModelRepositoryTests
    {
        [TestMethod]
        public async Task DeployModel_GivenAnExistingModel_ShouldDeployModelToDeploymentTargetFolder()
        {
            //Arrange
            var destinationFolder = @"C:\mlops";
            var localFileModelRepository = new LocalFileModelRepository(new FileSystem(), destinationFolder);

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");

            await localFileModelRepository.UploadModelAsync(registeredModel.RunId, @"Data/model.txt");

            //Act
            var deployedPath = localFileModelRepository.DeployModel(deploymentTarget, registeredModel);

            //Assert
            File.Exists(deployedPath).Should().BeTrue();
        }
    }
}
