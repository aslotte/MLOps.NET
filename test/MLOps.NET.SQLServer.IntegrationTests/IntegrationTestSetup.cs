using Microsoft.EntityFrameworkCore;
using MLOps.NET.Extensions;
using MLOps.NET.SQLServer.IntegrationTests.Constants;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Configuration;

namespace MLOps.NET.SQLServer.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLServer(configuration[ConfigurationKeys.ConnectionString])
                .UseContainerRegistry(
                configuration[ConfigurationKeys.RegistryName],
                configuration[ConfigurationKeys.Username],
                configuration[ConfigurationKeys.Password])
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            var options = new DbContextOptionsBuilder()
                .UseSqlServer(configuration[ConfigurationKeys.ConnectionString])
                .Options;

            return new DbContextFactory(() => new MLOpsSQLDbContext(options)).CreateDbContext();
        }
    }
}
