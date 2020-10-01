using Amazon.S3;
using Amazon.S3.Model;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
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

        public S3BucketModelRepository(IAmazonS3 s3Client, IModelPathGenerator modelPathGenerator)
        {
            this.s3Client = s3Client;
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

            destination.Position = 0;
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, Experiment experiment)
        {
            var deploymentPath = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, experiment.ExperimentName);

            await CreateBucketAsync(this.s3Client, deploymentRepositoryBucket, isPublic: true);

            var copyObjectRequest = new CopyObjectRequest
            {
                SourceBucket = modelRepositoryBucket,
                SourceKey = this.modelPathGenerator.GetModelName(registeredModel.RunId),
                DestinationBucket = deploymentRepositoryBucket,
                DestinationKey = deploymentPath
            };

            await this.s3Client.CopyObjectAsync(copyObjectRequest);

            return GetDeploymentUri(experiment, deploymentTarget);
        }

        public string GetDeploymentUri(Experiment experiment, DeploymentTarget deploymentTarget)
        {
            var deploymentPath = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, experiment.ExperimentName);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = deploymentRepositoryBucket,
                Key = deploymentPath,
                Expires = DateTime.Now.AddYears(5),
                Protocol = Protocol.HTTP
            };

            return this.s3Client.GetPreSignedURL(request);
        }

        private async Task CreateBucketAsync(IAmazonS3 s3Client, string bucketName, bool isPublic = false)
        {
            var bucketExists = await DoesS3BucketExists(s3Client, bucketName);
            if (!bucketExists)
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.Private
                };

                await s3Client.PutBucketAsync(request);
            }
        }

        private async Task<bool> DoesS3BucketExists(IAmazonS3 s3Client, string bucketName)
        {
            var buckets = await s3Client.ListBucketsAsync();

            return buckets.Buckets.Any(b => b.BucketName == bucketName);
        }
    }
}
