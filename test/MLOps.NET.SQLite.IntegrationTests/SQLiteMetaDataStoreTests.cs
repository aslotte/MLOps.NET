using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.IntegrationTests;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using Moq;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class SQLiteMetaDataStoreTests : MetaDataStoreTests
    {
        [TestInitialize]
        public void Initialize()
        {
            base.sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLite()
                .Build();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options;

            var contextFactory = new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating);

            await base.TearDown(contextFactory.CreateDbContext());
        }
    }
}
