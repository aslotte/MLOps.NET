using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLServer.IntegrationTests.Constants;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Configuration;
using Moq;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLServer(configuration[ConfigurationKeys.ConnectionString])
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            var options = new DbContextOptionsBuilder()
                .UseSqlServer(configuration[ConfigurationKeys.ConnectionString])
                .Options;

            return new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating).CreateDbContext();
        }
    }
}
