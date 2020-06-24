using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountModelRepository : IModelRepository
    {
        private readonly AmazonS3Client amazonS3Client;
        private string bucketName;

        public StorageAccountModelRepository(string awsAccessKeyId, string awsSecretAccessKey, string regionName, string bucketName)
        {
            var region = RegionEndpoint.GetBySystemName(regionName);
            this.bucketName = bucketName;
            this.amazonS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, region);
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, bucketName);
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

        /// <summary>
        /// Downloads the model file from disk into the provided stream.
        /// </summary>
        /// <param name="runId">Run ID to download model for</param>
        /// <param name="destination">Destination stream to write model into</param>
        /// <returns>Task with result of download operation</returns>
        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            var downloadFileRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = runId.ToString()
            };
            var response = await amazonS3Client.GetObjectAsync(downloadFileRequest);
            using (var stream = response.ResponseStream)
                await stream.CopyToAsync(destination);
        }
    }
}
