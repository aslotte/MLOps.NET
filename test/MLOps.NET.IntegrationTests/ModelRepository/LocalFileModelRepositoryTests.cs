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
        [TestMethod]
        public async Task DeployModel_GivenAnExistingModel_ShouldDeployModelToDeploymentTargetFolder()
        {
            //Arrange
            var destinationFolder = @"C:\mlops";
            var sut = new LocalFileModelRepository(new FileSystem(), new ModelPathGenerator(), destinationFolder);

            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");

            await sut.UploadModelAsync(registeredModel.RunId, @"Data/model.txt");

            //Act
            var deployedPath = await sut.DeployModelAsync(deploymentTarget, registeredModel);

            //Assert
            File.Exists(deployedPath).Should().BeTrue();
        }
    }
}
