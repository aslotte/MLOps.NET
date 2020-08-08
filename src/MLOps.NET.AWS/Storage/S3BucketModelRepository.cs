using Amazon.S3;
using Amazon.S3.Model;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class S3BucketModelRepository : IModelRepository
    {
        private readonly IAmazonS3 s3Client;
        private readonly IModelPathGenerator modelPathGenerator;
        private readonly string modelRepositoryBucket = "model-repository";
        private readonly string deploymentRepositoryBucket = "deployment";

        public S3BucketModelRepository(IAmazonS3 amazonS3Client, IModelPathGenerator modelPathGenerator)
        {
            this.s3Client = amazonS3Client;
            this.modelPathGenerator = modelPathGenerator;
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            await CreateBucketAsync(this.s3Client, modelRepositoryBucket);

            var fileUploadRequest = new PutObjectRequest
            {
                BucketName = modelRepositoryBucket,
                Key = this.modelPathGenerator.GetModelName(runId),
                FilePath = filePath,
                ContentType = "application/zip"
            };

            await s3Client.PutObjectAsync(fileUploadRequest);
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            var downloadFileRequest = new GetObjectRequest
            {
                BucketName = modelRepositoryBucket,
                Key = this.modelPathGenerator.GetModelName(runId)
            };
            var response = await s3Client.GetObjectAsync(downloadFileRequest);
            if (response == null)
            {
                throw new FileNotFoundException($"The model file for run id {runId} was not found");
            }

            using var stream = response.ResponseStream;
            await stream.CopyToAsync(destination);
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            await CreateBucketAsync(this.s3Client, deploymentRepositoryBucket, isPublic: true);

            var copyObjectRequest = new CopyObjectRequest
            {
                SourceBucket = modelRepositoryBucket,
                SourceKey = this.modelPathGenerator.GetModelName(registeredModel.RunId),
                DestinationBucket = deploymentRepositoryBucket,
                DestinationKey = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, registeredModel)
            };

            await this.s3Client.CopyObjectAsync(copyObjectRequest);

            return GetDeploymentUri(deploymentTarget, registeredModel);
        }

        public string GetDeploymentUri(Deployment deployment)
        {
            return GetDeploymentUri(deployment.DeploymentTarget, deployment.RegisteredModel);
        }

        private string GetDeploymentUri(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = deploymentRepositoryBucket,
                Key = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, registeredModel),
                Expires = DateTime.Now.AddYears(5),
                Protocol = Protocol.HTTP
            };

            return this.s3Client.GetPreSignedURL(request);
        }

        private async Task CreateBucketAsync(IAmazonS3 amazonS3Client, string bucketName, bool isPublic = false)
        {
            var bucketExists = await DoesS3BucketExists(amazonS3Client, bucketName);
            if (!bucketExists)
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.Private
                };

                await amazonS3Client.PutBucketAsync(request);
            }
        }

        private async Task<bool> DoesS3BucketExists(IAmazonS3 amazonS3Client, string bucketName)
        {
            var buckets = await amazonS3Client.ListBucketsAsync();

            return buckets.Buckets.Any(b => b.BucketName == bucketName);
        }
    }
}
