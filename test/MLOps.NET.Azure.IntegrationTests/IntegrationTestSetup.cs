using Microsoft.EntityFrameworkCore;
using MLOps.NET.Azure.IntegrationTests.Constants;
using MLOps.NET.Extensions;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Tests.Common.Configuration;

namespace MLOps.NET.Azure.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseCosmosDb(
                configuration[ConfigurationKeys.CosmosEndPoint],
                configuration[ConfigurationKeys.CosmosAccountKey])
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
                .UseCosmos(configuration[ConfigurationKeys.CosmosEndPoint],
                configuration[ConfigurationKeys.CosmosAccountKey], "MLOpsNET")
                .Options;

            var contextFactory = new DbContextFactory(() => new MLOpsCosmosDbContext(options));
            var context = contextFactory.CreateDbContext();

            return contextFactory.CreateDbContext();
        }
    }
}
