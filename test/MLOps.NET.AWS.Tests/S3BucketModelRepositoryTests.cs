using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod]
        public async Task UploadModelAsync_ShouldSaveFileInS3Bucket()
        {
            // Arrange
            var mockAmzonClient = new Mock<IAmazonS3>();
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
            var sut = new S3BucketModelRepository(mockAmzonClient.Object,"model-repository");

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockAmzonClient.Verify(a => a.PutObjectAsync(It.Is<PutObjectRequest>(o => o.BucketName == "model-repository"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UploadModelAsync_ShouldCreateS3BucketIfNotExists()
        {
            // Arrange
            var mockAmzonClient = new Mock<IAmazonS3>();
            mockAmzonClient.Setup(a => a.ListBucketsAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new ListBucketsResponse
                           {
                               Buckets = new List<S3Bucket>()
                           });

            var sut = new S3BucketModelRepository(mockAmzonClient.Object, "model-repository");

            // Act
            await sut.UploadModelAsync(new Guid(), "model.zip");

            // Assert
            mockAmzonClient.Verify(a => a.PutBucketAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
