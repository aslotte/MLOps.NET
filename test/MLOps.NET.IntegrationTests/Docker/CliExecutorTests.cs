using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests.Docker
{
    [TestClass]
    public class CliExecutorTests
    {
        private CliExecutor sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sut = new CliExecutor(new DockerSettings());
        }

        [TestMethod]
        public async Task CreateTemplateProject_ShouldCreateTemplateProject()
        {
            //Arrange
            await sut.UninstallTemplatePackage();
            await sut.InstallTemplatePackage();

            //Act
            await sut.CreateTemplateProject();

            //Assert
            File.Exists("image/ML.NET.Web.Embedded.csproj").Should().BeTrue();
        }
    }
}
