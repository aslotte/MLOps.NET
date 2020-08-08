using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Azure.IntegrationTests.Constants;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using MLOps.NET.Tests.Common.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MLOps.NET.Azure.IntegrationTests
{
    [TestCategory("IntegrationTestsAzure")]
    [TestClass]
    public class StorageAccountModelRepositoryTests
    {
        private StorageAccountModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();
            var connectionString = configuration[ConfigurationKeys.StorageAccount];

            var modelRepositoryClient = new BlobContainerClient(connectionString, "model-repository");
            var deploymentClient = new BlobContainerClient(connectionString, "deployment");

            modelRepositoryClient.CreateIfNotExists(PublicAccessType.None);
            deploymentClient.CreateIfNotExists(PublicAccessType.Blob);

            sut = new StorageAccountModelRepository(modelRepositoryClient, deploymentClient, new ModelPathGenerator());  
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldSucceed()
        {
            //Arrange       
            var runId = Guid.NewGuid();

            //Act
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            //Assert
            using var memoryStream = new MemoryStream();
            await sut.DownloadModelAsync(runId, memoryStream);

            memoryStream.Should().NotBeNull();
            memoryStream.Length.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task DeployModelAsync_ShouldDeployModelToTarget()
        {
            //Arrange
            var runId = Guid.NewGuid();
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            var registeredModel = new RegisteredModel
            {
                RunId = runId,
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");

            //Act
            var uri = await sut.DeployModelAsync(deploymentTarget, registeredModel);

            //Assert
            var client = new HttpClient();
            var response = await client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task DeployModelAsync_GivenADeployedModelAlreadyExist_ShouldOverwriteExistingModel()
        {
            //Arrange
            var client = new HttpClient();

            var runId = Guid.NewGuid();
            var runId1 = Guid.NewGuid();

            await sut.UploadModelAsync(runId, @"Data/model.txt");
            await sut.UploadModelAsync(runId1, @"Data/model.txt");

            var registeredModel = new RegisteredModel
            {
                RunId = runId,
                Experiment = new Experiment("ExperimentName")
            };

            var registeredModel1 = new RegisteredModel
            {
                RunId = runId1,
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");

            var firstModelUri = await sut.DeployModelAsync(deploymentTarget, registeredModel1);
            var firstModelResponse = await client.GetAsync(firstModelUri);

            var firstModelUpdateTime = firstModelResponse.Content.Headers.LastModified;

            //Act
            Thread.Sleep(60000);
            var uri = await sut.DeployModelAsync(deploymentTarget, registeredModel);

            //Assert
            var response = await client.GetAsync(uri);

            var secondModelUpdateTime = response.Content.Headers.LastModified;
            firstModelUpdateTime.Value.Ticks.Should().BeLessThan(secondModelUpdateTime.Value.Ticks);
        }

        [TestMethod]
        public async Task GetDeploymentUri_GivenADeployedModel_ShouldReturnAValidUri()
        {
            //Arrange
            var runId = Guid.NewGuid();
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            var registeredModel = new RegisteredModel
            {
                RunId = runId,
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");
            await sut.DeployModelAsync(deploymentTarget, registeredModel);

            var deployment = new Deployment
            {
                RegisteredModel = registeredModel,
                DeploymentTarget = deploymentTarget
            };

            //Act
            var uri = sut.GetDeploymentUri(deployment);

            //Assert
            var client = new HttpClient();
            var response = await client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
        }
    }
}
