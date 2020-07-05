using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace MLOps.NET.Storage
{
    internal sealed class S3BucketModelRepository : IModelRepository
    {
        private readonly IAmazonS3 amazonS3Client;
        private string bucketName;

        public S3BucketModelRepository(IAmazonS3 amazonS3Client, string bucketName)
        {           
            this.bucketName = bucketName;
            this.amazonS3Client = amazonS3Client;
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            var bucketExists = await DoesS3BucketExists(bucketName);
            if (!bucketExists)
            {
                await this.amazonS3Client.PutBucketAsync(bucketName);
            }

            var fileUploadRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = runId.ToString(),
                FilePath = filePath,
                ContentType = "application/zip"
            };

            await amazonS3Client.PutObjectAsync(fileUploadRequest);
        }

        private async Task<bool> DoesS3BucketExists(string bucketName)
        {                
            return (await amazonS3Client.ListBucketsAsync()).Buckets.Any(b => b.BucketName == bucketName);                
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            var downloadFileRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = runId.ToString()
            };
            var response = await amazonS3Client.GetObjectAsync(downloadFileRequest);
            if (response == null)
            {
                throw new FileNotFoundException($"The model file for run id {runId} was not found");
            }

            using (var stream = response.ResponseStream)
            {
                await stream.CopyToAsync(destination);
            }
        }
    }
}
