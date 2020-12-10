using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using Moq;
using System;
using System.Collections.Generic;
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
        private RegisteredModel registeredModel;
        private Experiment experiment;

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

            this.registeredModel = new RegisteredModel
            {
                Version = 1,
                RunId = Guid.NewGuid()
            };

            this.experiment = new Experiment("Test");
        }

        [TestMethod]
        public async Task BuildImage_ShouldInstallTemplates()
        {
            //Arrange
            experiment.Runs = new List<Run>
            {
                new Run(experiment.ExperimentId) 
                { 
                    RunId = registeredModel.RunId
                }
            };

            //Act
            await sut.BuildImage(experiment, registeredModel, new MemoryStream(), GetSchema);

            //Assert
            mockCliExecutor.Verify(x => x.InstallTemplatePackage(), Times.Once());
        }

        [TestMethod]
        public async Task BuildImage_ShouldCreateProjectTemplate()
        {
            //Arrange
            experiment.Runs = new List<Run> 
            { 
                new Run(experiment.ExperimentId) 
                { 
                    RunId = registeredModel.RunId 
                } 
            };

            //Act
            await sut.BuildImage(experiment, registeredModel, new MemoryStream(), GetSchema);

            //Assert
            mockCliExecutor.Verify(x => x.CreateTemplateProject(), Times.Once());
        }

        [TestMethod]
        public async Task BuildImage_ShouldRunDockerBuild()
        {
            //Arrange
            experiment.Runs = new List<Run>
            {
                new Run(experiment.ExperimentId) 
                { 
                    RunId = registeredModel.RunId
                }
            };

            //Act
            await sut.BuildImage(experiment, registeredModel, new MemoryStream(), GetSchema);

            //Assert
            var imageName = $"{dockerSettings.RegistryName}/Test:{registeredModel.Version}".ToLower();
            mockCliExecutor.Verify(x => x.RunDockerBuild(imageName), Times.Once());
        }

        [TestMethod]
        public async Task PushImage_ShouldRunDockerLogin()
        {
            //Arrange

            //Act
            await sut.PushImage("Test", registeredModel);

            //Assert
            mockCliExecutor.Verify(x => x.RunDockerLogin(dockerSettings), Times.Once());
        }

        [TestMethod]
        public async Task PushImage_ShouldRunDockerPush()
        {
            //Arrange

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

            //Act
            var imageTag = sut.ComposeImageName("Test Experiment Name", registeredModel);

            //Assert
            imageTag.Should().Be("registry/testexperimentname:1");
        }

        [TestMethod]
        public async Task BuildImage_ShouldAddPackageDependencies()
        {
            //Arrange
            var packageDependencies = new List<PackageDependency>
            {
                new PackageDependency
                {
                    Name = "Microsoft.ML",
                    Version = "1.5.2"
                }
            };

            experiment.Runs = new List<Run>
            {
                new Run(experiment.ExperimentId) 
                { 
                    RunId = registeredModel.RunId,
                    PackageDepedencies = packageDependencies
                }
            };

            //Act
            await sut.BuildImage(experiment, registeredModel, new MemoryStream(), GetSchema);

            //Assert
            mockCliExecutor.Verify(x => x.AddPackageDependencies(packageDependencies), Times.Once());
        }

        private (string ModelInput, string ModelOutput) GetSchema() => ("input", "output");
    }
}
