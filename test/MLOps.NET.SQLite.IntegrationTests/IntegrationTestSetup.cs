using Microsoft.EntityFrameworkCore;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite.IntegrationTests.Constants;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Configuration;

namespace MLOps.NET.SQLite.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLite()
                .UseContainerRegistry(
                configuration[ConfigurationKeys.RegistryName], 
                configuration[ConfigurationKeys.Username], 
                configuration[ConfigurationKeys.Password])
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options;

            return new DbContextFactory(() => new MLOpsSQLiteDbContext(options)).CreateDbContext();
        }
    }
}
