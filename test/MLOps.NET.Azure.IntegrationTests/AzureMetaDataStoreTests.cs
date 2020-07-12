using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Azure.IntegrationTests.Constants;
using MLOps.NET.Azure.Storage;
using MLOps.NET.IntegrationTests;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Configuration;
using Moq;
using System.Threading.Tasks;

namespace MLOps.NET.Azure.IntegrationTests
{
    [TestClass]
    [TestCategory("IntegrationTestCosmosDb")]
    public class AzureMetaDataStoreTests : MetaDataStoreTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseCosmosDb(configuration[ConfigurationKeys.CosmosEndPoint],
                configuration[ConfigurationKeys.CosmosAccountKey])
                .Build();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            var options = new DbContextOptionsBuilder()
                .UseCosmos(configuration[ConfigurationKeys.CosmosEndPoint],
                configuration[ConfigurationKeys.CosmosAccountKey], "MLOpsNET")
                .Options;

            var contextFactory = new DbContextFactory(options, CosmosEntityConfigurator.OnModelCreating);
            var context = contextFactory.CreateDbContext();

            await base.TearDown(contextFactory.CreateDbContext());
        }
    }
}
