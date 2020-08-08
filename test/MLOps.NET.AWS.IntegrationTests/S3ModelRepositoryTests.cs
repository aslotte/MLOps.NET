using Amazon;
using Amazon.S3;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.AWS.IntegrationTests
{
    [TestCategory("IntegrationTestsAWS")]
    [TestClass]
    public class S3ModelRepositoryTests
    {
        private S3BucketModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var s3Client = new AmazonS3Client("test","test", new AmazonS3Config
            { 
                ServiceURL = "http://localhost:9090",
                ForcePathStyle = true,
            });

            sut = new S3BucketModelRepository(s3Client);
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
