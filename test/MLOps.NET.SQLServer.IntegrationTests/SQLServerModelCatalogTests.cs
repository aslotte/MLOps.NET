using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.IntegrationTests;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    [TestCategory("IntegrationTestSqlServer")]
    [TestClass]
    public class SQLServerModelCatalogTests : ModelCatalogTests
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
