using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.Interfaces;
using Moq;

namespace MLOps.NET.SQLite.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            return new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLite()
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options;

            return new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating).CreateDbContext();
        }
    }
}
