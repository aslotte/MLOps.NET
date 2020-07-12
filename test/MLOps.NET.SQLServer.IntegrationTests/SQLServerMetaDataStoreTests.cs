using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.IntegrationTests;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using Moq;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    [TestClass]
    [TestCategory("IntegrationTestSqlServer")]
    public class SQLServerMetaDataStoreTests : MetaDataStoreTests
    {
        private const string connectionString = "Server=localhost,1433;Database=MLOpsNET_IntegrationTests;User Id=sa;Password=MLOps4TheWin!;";

        [TestInitialize]
        public void Initialize()
        {
            sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLServer(connectionString)
                .Build();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            var contextFactory = new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating);

            await base.TearDown(contextFactory.CreateDbContext());
        }
    }
}
