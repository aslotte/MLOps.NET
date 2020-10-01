using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Services;
using MLOps.NET.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MLOps.NET.AWS.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class S3BucketModelRepositoryTests
    {
        private Mock<IAmazonS3> mockAmzonClient;
        private S3BucketModelRepository sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockAmzonClient = new Mock<IAmazonS3>();
            this.sut = new S3BucketModelRepository(mockAmzonClient.Object, new ModelPathGenerator());
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldSaveFileInS3Bucket()
        {
            // Arrange
            mockAmzonClient.Setup(a => a.ListBucketsAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new ListBucketsResponse
                           {
                               Buckets = new List<S3Bucket>()
                               {
                                   new S3Bucket
                                   {
                                       BucketName = "model-repository"
                                   }
                               }
                           });

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockAmzonClient.Verify(a => a.PutObjectAsync(It.Is<PutObjectRequest>(o => o.BucketName == "model-repository"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldCreateS3BucketIfNotExists()
        {
            // Arrange
            mockAmzonClient.Setup(a => a.ListBucketsAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new ListBucketsResponse
                           {
                               Buckets = new List<S3Bucket>()
                           });

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockAmzonClient.Verify(a => a.PutBucketAsync(It.Is<PutBucketRequest>(x => x.BucketName == "model-repository" && x.CannedACL == S3CannedACL.Private), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
