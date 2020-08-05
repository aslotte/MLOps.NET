using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.IntegrationTests;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    [TestClass]
    [TestCategory("IntegrationTestSqlServer")]
    public class SQLServerDeploymentCatalogTests : DeploymentCatalogTests
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
    }
}
