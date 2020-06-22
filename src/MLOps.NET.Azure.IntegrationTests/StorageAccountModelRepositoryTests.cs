using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Azure.IntegrationTests
{
    [TestClass]
    public class StorageAccountModelRepositoryTests
    {
        private StorageAccountModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            sut = new StorageAccountModelRepository("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;");
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldSucceed()
        {
            //Arrange       
            var runId = Guid.NewGuid();

            //Act
            await sut.UploadModelAsync(runId, @"Data\model.txt");

            //Assert
            using var memoryStream = new MemoryStream();
            await sut.DownloadModelAsync(runId, memoryStream);

            memoryStream.Should().NotBeNull();
            memoryStream.Length.Should().BeGreaterThan(0);
        }
    }
}
