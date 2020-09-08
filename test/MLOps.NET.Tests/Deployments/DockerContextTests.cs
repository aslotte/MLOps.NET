using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using Moq;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class DockerContextTests
    {
        private DockerSettings dockerSettings;
        private Mock<ICliExecutor> mockCliExecutor;
        private DockerContext sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dockerSettings = new DockerSettings
            {
                RegistryName = "registry"
            };
            this.mockCliExecutor = new Mock<ICliExecutor>();
            this.sut = new DockerContext(mockCliExecutor.Object, new FileSystem(), dockerSettings);
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
            await sut.BuildImage("Test", registeredModel, new MemoryStream());

            //Assert
            mockCliExecutor.Verify(x => x.InstallTemplatePackage(), Times.Once());
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
            await sut.BuildImage("Test", registeredModel, new MemoryStream());

            //Assert
            mockCliExecutor.Verify(x => x.CreateTemplateProject(), Times.Once());
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
            await sut.BuildImage("Test", registeredModel, new MemoryStream());

            //Assert
            var tagName = $"{dockerSettings.RegistryName}/Test:{registeredModel.Version}";
            mockCliExecutor.Verify(x => x.RunDockerBuild(tagName), Times.Once());
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
            mockCliExecutor.Verify(x => x.RunDockerLogin(), Times.Once());
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
            var tagName = $"{dockerSettings.RegistryName}/Test:{registeredModel.Version}";
            mockCliExecutor.Verify(x => x.RunDockerPush(tagName), Times.Once());
        }
    }
}
