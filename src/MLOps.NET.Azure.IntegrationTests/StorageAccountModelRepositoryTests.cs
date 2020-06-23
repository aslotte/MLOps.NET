using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Azure.IntegrationTests.Constants;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Azure.IntegrationTests
{
    [TestCategory("IntegrationTestsAzure")]
    [TestClass]
    public class StorageAccountModelRepositoryTests
    {
        private StorageAccountModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            sut = new StorageAccountModelRepository(configuration[ConfigurationKeys.StorageAccount]);
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldSucceed()
        {
            //Arrange       
            var runId = Guid.NewGuid();

            //Act
            await sut.UploadModelAsync(runId, @"Data/model.txt");

            //Assert
            using var memoryStream = new MemoryStream();
            await sut.DownloadModelAsync(runId, memoryStream);

            memoryStream.Should().NotBeNull();
            memoryStream.Length.Should().BeGreaterThan(0);
        }
    }
}
