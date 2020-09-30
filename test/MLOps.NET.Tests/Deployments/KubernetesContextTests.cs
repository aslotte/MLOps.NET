using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using Moq;
using System.Threading.Tasks;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class KubernetesContextTests
    {
        private DockerSettings dockerSettings;
        private KubernetesSettings kubernetesSettings;
        private Mock<ICliExecutor> mockCliExecutor;
        private Mock<IManifestParameterizator> mockManifestParameterizator;
        private KubernetesContext sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dockerSettings = new DockerSettings
            {
                RegistryName = "registry"
            };

            this.kubernetesSettings = new KubernetesSettings();

            this.mockCliExecutor = new Mock<ICliExecutor>();
            this.mockManifestParameterizator = new Mock<IManifestParameterizator>();

            this.sut = new KubernetesContext(mockCliExecutor.Object, kubernetesSettings, dockerSettings, mockManifestParameterizator.Object);
        }

        [TestMethod]
        public async Task CreateNamespace_ShouldCreateCorrectName()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");

            //Act
            var name = await this.sut.CreateNamespaceAsync(experimentName, deploymentTarget);

            //Assert
            var expectedName = $"{experimentName}-{deploymentTarget.Name}".ToLower();
            name.Should().Be(expectedName);
        }

        [TestMethod]
        public async Task CreateNamespace_ShouldCallCliExecutor()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");

            //Act
            var name = await this.sut.CreateNamespaceAsync(experimentName, deploymentTarget);

            //Assert
            var expectedName = $"{experimentName}-{deploymentTarget.Name}".ToLower();
            this.mockCliExecutor.Verify(x => x.CreateNamespaceAsync(expectedName, kubernetesSettings), Times.Once());
        }

        [TestMethod]
        public async Task DeployContainerAsync_ShouldCallCreateImagePullSecret()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");
            var imageName = "image123";
            var namespaceName = "experiment-test";

            //Act
            await this.sut.DeployContainerAsync(experimentName, imageName, namespaceName);

            //Assert
            this.mockCliExecutor.Verify(x => x.CreateImagePullSecret(kubernetesSettings, dockerSettings, namespaceName), Times.Once());
        }

        [TestMethod]
        public async Task DeployContainerAsync_ShouldCallKubectlApplyForDeployManifest()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");
            var imageName = "image123";
            var namespaceName = "experiment-test";

            //Act
            await this.sut.DeployContainerAsync(experimentName, imageName, namespaceName);

            //Assert
            this.mockCliExecutor.Verify(x => x.KubctlApplyAsync(kubernetesSettings, "deploy.yml"), Times.Once());
        }

        [TestMethod]
        public async Task DeployContainerAsync_ShouldCallKubectlApplyForServiceManifest()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");
            var imageName = "image123";
            var namespaceName = "experiment-test";

            //Act
            await this.sut.DeployContainerAsync(experimentName, imageName, namespaceName);

            //Assert
            this.mockCliExecutor.Verify(x => x.KubctlApplyAsync(kubernetesSettings, "service.yml"), Times.Once());
        }

        [TestMethod]
        public async Task DeployContainerAsync_ShouldCallParameterizeServiceManifest()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");
            var imageName = "image123";
            var namespaceName = "experiment-test";

            //Act
            await this.sut.DeployContainerAsync(experimentName, imageName, namespaceName);

            //Assert
            this.mockManifestParameterizator.Verify(x => x.ParameterizeServiceManifest(experimentName, namespaceName), Times.Once());
        }

        [TestMethod]
        public async Task DeployContainerAsync_ShouldCallParameterizeDeployManifest()
        {
            //Arrange
            var experimentName = "experiment";
            var deploymentTarget = new DeploymentTarget("Test");
            var imageName = "image123";
            var namespaceName = "experiment-test";

            //Act
            await this.sut.DeployContainerAsync(experimentName, imageName, namespaceName);

            //Assert
            this.mockManifestParameterizator.Verify(x => x.ParameterizeDeploymentManifest(experimentName, imageName, namespaceName), Times.Once());
        }
    }
}
