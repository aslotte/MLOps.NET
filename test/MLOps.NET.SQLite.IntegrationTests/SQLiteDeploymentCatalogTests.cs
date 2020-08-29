using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.IntegrationTests;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class SQLiteDeploymentCatalogTests : DeploymentCatalogTests
    {
        [TestInitialize]
        public void Initialize()
        {
            sut = IntegrationTestSetup.Initialize();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            var context = IntegrationTestSetup.CreateDbContext();

            await base.TearDown(context);
        }

        [TestMethod]
        public async Task DeployModelToContainerAsync_ShouldDeployModelToRegistry()
        {
            //Arrange
            var registeredModel = await CreateRegisteredModel();
            var deploymentTarget = await CreateDeploymentTarget("Prod");

            //Act
            var deployment = await sut.Deployment.DeployModelToContainerAsync(deploymentTarget, registeredModel, "MLOps.NET team");

            //Assert
        }
    }
}
