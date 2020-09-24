using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes;
using MLOps.NET.Kubernetes.Settings;
using Moq;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class KubernetesContextTests
    {
        private DockerSettings dockerSettings;
        private KubernetesSettings kubernetesSettings;
        private Mock<ICliExecutor> mockCliExecutor;
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
            this.sut = new KubernetesContext(mockCliExecutor.Object, kubernetesSettings, dockerSettings, new FileSystem());
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
            var expectedName = $"{experimentName}-{deploymentTarget.Name}";
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
            var expectedName = $"{experimentName}-{deploymentTarget.Name}";
            this.mockCliExecutor.Verify(x => x.CreateNamespaceAsync(expectedName, kubernetesSettings), Times.Once());
        }
    }
}
