using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using Moq;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class DockerContextTests
    {
        private DockerSettings dockerSettings;
        private Mock<ICliExecutor> mockCliExecutor;
        private MockFileSystem mockFileSystem;
        private DockerContext sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dockerSettings = new DockerSettings
            {
                RegistryName = "registry"
            };
            this.mockCliExecutor = new Mock<ICliExecutor>();
            this.mockFileSystem = new MockFileSystem();
            this.sut = new DockerContext(mockCliExecutor.Object, mockFileSystem, dockerSettings);
        }

        [TestMethod]
        public async Task BuildImage_ShouldInstallTemplates()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            await sut.BuildImage("Test", registeredModel, new MemoryStream(), GetSchema);

            //Assert
            mockCliExecutor.Verify(x => x.InstallTemplatePackage(dockerSettings), Times.Once());
        }

        [TestMethod]
        public async Task BuildImage_ShouldCreateProjectTemplate()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            await sut.BuildImage("Test", registeredModel, new MemoryStream(), GetSchema);

            //Assert
            mockCliExecutor.Verify(x => x.CreateTemplateProject(dockerSettings), Times.Once());
        }

        [TestMethod]
        public async Task BuildImage_ShouldRunDockerBuild()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            await sut.BuildImage("Test", registeredModel, new MemoryStream(), GetSchema);

            //Assert
            var imageName = $"{dockerSettings.RegistryName}/Test:{registeredModel.Version}".ToLower();
            mockCliExecutor.Verify(x => x.RunDockerBuild(dockerSettings, imageName), Times.Once());
        }

        [TestMethod]
        public async Task PushImage_ShouldRunDockerLogin()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            await sut.PushImage("Test", registeredModel);

            //Assert
            mockCliExecutor.Verify(x => x.RunDockerLogin(dockerSettings), Times.Once());
        }

        [TestMethod]
        public async Task PushImage_ShouldRunDockerPush()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            await sut.PushImage("Test", registeredModel);

            //Assert
            var tagName = $"{dockerSettings.RegistryName}/Test:{registeredModel.Version}".ToLower();
            mockCliExecutor.Verify(x => x.RunDockerPush(tagName), Times.Once());
        }

        [TestMethod]
        public void ComposeTag_ShouldRemoveSpacesAndReturnToLower()
        {
            //Arrange
            var registeredModel = new RegisteredModel
            {
                Version = 1
            };

            //Act
            var imageTag = sut.ComposeImageName("Test Experiment Name", registeredModel);

            //Assert
            imageTag.Should().Be("registry/testexperimentname:1");
        }

        private (string ModelInput, string ModelOutput) GetSchema() => ("input", "output");
    }
}
