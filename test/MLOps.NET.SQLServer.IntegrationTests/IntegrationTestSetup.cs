using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.Interfaces;
using Moq;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        private const string connectionString = "Server=localhost,1433;Database=MLOpsNET_IntegrationTests;User Id=sa;Password=MLOps4TheWin!;";

        internal static IMLOpsContext Initialize()
        {
            return new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLServer(connectionString)
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            return new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating).CreateDbContext();
        }
    }
}
