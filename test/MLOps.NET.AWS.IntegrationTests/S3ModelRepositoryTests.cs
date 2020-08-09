using Amazon.S3;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MLOps.NET.AWS.IntegrationTests
{
    [TestCategory("IntegrationTestsAWS")]
    [TestClass]
    public class S3ModelRepositoryTests
    {
        private S3BucketModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var s3Client = new AmazonS3Client("test", "test", new AmazonS3Config
            {
                ServiceURL = "http://localhost:9090",
                ForcePathStyle = true,
            });

            sut = new S3BucketModelRepository(s3Client, new ModelPathGenerator());
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
        public async Task DeployModelAsync_ShouldDeployModel()
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

        [TestMethod]
        public async Task DeployModelAsync_GivenADeployedModelAlreadyExist_ShouldOverwriteExistingModel()
        {
            //Arrange
            var client = new HttpClient();

            var deploymentTarget = new DeploymentTarget("Test");

            var firstModelUri = await DeployModelAsync(deploymentTarget);

            var model1 = await client.GetAsync(firstModelUri);
            var firstModelUpdateTime = model1.Content.Headers.LastModified;

            Thread.Sleep(60000);

            //Act
            var uri = await DeployModelAsync(deploymentTarget);

            //Assert
            var model2 = await client.GetAsync(uri);
            var secondModelUpdateTime = model2.Content.Headers.LastModified;

            firstModelUpdateTime.Value.Ticks.Should().BeLessThan(secondModelUpdateTime.Value.Ticks);
        }

        private async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget)
        {
            var runId = Guid.NewGuid();

            await sut.UploadModelAsync(runId, @"Data/model.txt");

            var registeredModel = new RegisteredModel
            {
                RunId = runId,
                Experiment = new Experiment("ExperimentName")
            };
            return await sut.DeployModelAsync(deploymentTarget, registeredModel);
        }
    }
}
