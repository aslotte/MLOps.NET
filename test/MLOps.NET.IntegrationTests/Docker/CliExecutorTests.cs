using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests.Docker
{
    [TestCategory("Integration")]
    [TestClass]
    public class CliExecutorTests
    {
        private DockerSettings dockerSettings;
        private CliExecutor sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dockerSettings = new DockerSettings();
            this.sut = new CliExecutor();
        }

        [TestMethod]
        public async Task CreateTemplateProject_ShouldCreateTemplateProject()
        {
            //Arrange
            await sut.UninstallTemplatePackage();
            await sut.InstallTemplatePackage(dockerSettings);

            //Act
            await sut.CreateTemplateProject(dockerSettings);

            //Assert
            File.Exists("image/ML.NET.Web.Embedded.csproj").Should().BeTrue();
        }
    }
}
