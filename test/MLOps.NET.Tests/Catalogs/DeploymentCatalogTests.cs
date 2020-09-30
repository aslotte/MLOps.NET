using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Data;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class DeploymentCatalogTests
    {
        private DeploymentCatalog sut;
        private Mock<IDeploymentRepository> deploymentRepositoryMock;
        private Mock<IModelRepository> modelRepositoryMock;
        private Mock<IExperimentRepository> experimentRepositoryMock;
        private Mock<IDockerContext> dockerContextMock;
        private Mock<IKubernetesContext> kubernetesContextMock;
        private Mock<ISchemaGenerator> schemaGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.deploymentRepositoryMock = new Mock<IDeploymentRepository>();
            this.modelRepositoryMock = new Mock<IModelRepository>();
            this.experimentRepositoryMock = new Mock<IExperimentRepository>();
            this.dockerContextMock = new Mock<IDockerContext>();
            this.kubernetesContextMock = new Mock<IKubernetesContext>();
            this.schemaGeneratorMock = new Mock<ISchemaGenerator>();

            schemaGeneratorMock.Setup(x => x.GenerateDefinition<ModelInput>("ModelInput")).Returns("input");
            schemaGeneratorMock.Setup(x => x.GenerateDefinition<ModelOutput>("ModelOutput")).Returns("output");

            this.sut = new DeploymentCatalog(deploymentRepositoryMock.Object, modelRepositoryMock.Object, experimentRepositoryMock.Object, dockerContextMock.Object, kubernetesContextMock.Object, schemaGeneratorMock.Object);
        }

        [ExpectedException(typeof(InvalidOperationException), "A container registry has not been configured. Please configure a container registry by calling UseContainerRegistry first")]
        [TestMethod]
        public async Task DeployModelToContainerAsync_GivenNoDockerContext_ShouldThrowException()
        {
            //Arrange
            this.sut = new DeploymentCatalog(deploymentRepositoryMock.Object, modelRepositoryMock.Object, experimentRepositoryMock.Object, null, null, null);

            var deploymentTarget = new DeploymentTarget("Test");
            var registeredModel = new RegisteredModel();

            //Act
            await sut.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "registeredBy");
        }

        [TestMethod]
        public async Task DeployModelToContainerAsync_ShouldCallBuildDockerImage()
        {
            //Arrange
            var deploymentTarget = new DeploymentTarget("Test");
            var registeredModel = new RegisteredModel();

            this.experimentRepositoryMock.Setup(x => x.GetExperiment(It.IsAny<Guid>()))
                .Returns(new Experiment(experimentName: "MyExperiment"));

            //Act
            await sut.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "registeredBy");

            //Arrange
            this.dockerContextMock.Verify(x => x.BuildImage("MyExperiment", registeredModel, It.IsAny<Stream>(), It.IsAny<Func<(string, string)>>()), Times.Once());
        }

        [TestMethod]
        public async Task DeployModelToContainerAsync_ShouldCallPushDockerImage()
        {
            //Arrange
            var deploymentTarget = new DeploymentTarget("Test");
            var registeredModel = new RegisteredModel();

            this.experimentRepositoryMock.Setup(x => x.GetExperiment(It.IsAny<Guid>()))
                .Returns(new Experiment(experimentName: "MyExperiment"));

            //Act
            await sut.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "registeredBy");

            //Arrange
            this.dockerContextMock.Verify(x => x.PushImage("MyExperiment", registeredModel), Times.Once());
        }

        [TestMethod]
        public async Task DeployModelToContainerAsync_ShouldCallCreateDeploymentAsync()
        {
            //Arrange
            var deploymentTarget = new DeploymentTarget("Test");
            var registeredModel = new RegisteredModel();

            this.experimentRepositoryMock.Setup(x => x.GetExperiment(It.IsAny<Guid>()))
                .Returns(new Experiment(experimentName: "MyExperiment"));

            //Act
            await sut.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "registeredBy");

            //Arrange
            this.deploymentRepositoryMock.Verify(x => x.CreateDeploymentAsync(deploymentTarget, registeredModel, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task DeployModelToContainerAsync_ShouldCallDeployContainerAsync()
        {
            //Arrange
            var deploymentTarget = new DeploymentTarget("Test");
            var registeredModel = new RegisteredModel();

            this.dockerContextMock.Setup(x => x.ComposeImageTag(It.IsAny<string>(), It.IsAny<RegisteredModel>()))
                .Returns("imagetag");

            this.experimentRepositoryMock.Setup(x => x.GetExperiment(It.IsAny<Guid>()))
                .Returns(new Experiment(experimentName: "MyExperiment"));

            this.kubernetesContextMock.Setup(x => x.CreateNamespaceAsync(It.IsAny<string>(), It.IsAny<DeploymentTarget>()))
                .Returns(Task.FromResult("myexperiment-test"));

            //Act
            await sut.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "registeredBy");

            //Arrange
            this.kubernetesContextMock.Verify(x => x.DeployContainerAsync("MyExperiment", "imagetag", "myexperiment-test"), Times.Once());
        }

        [TestMethod]
        public async Task BuildAndPushImageAsync_ShouldNotCallCreateDeploymentAsync()
        {
            //Arrange
            var registeredModel = new RegisteredModel();

            this.experimentRepositoryMock.Setup(x => x.GetExperiment(It.IsAny<Guid>()))
                .Returns(new Experiment(experimentName: "MyExperiment"));

            //Act
            await sut.BuildAndPushImageAsync<ModelInput, ModelOutput>(registeredModel);

            //Arrange
            this.deploymentRepositoryMock.Verify(x => x.CreateDeploymentAsync(It.IsAny<DeploymentTarget>(), registeredModel, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }
}
