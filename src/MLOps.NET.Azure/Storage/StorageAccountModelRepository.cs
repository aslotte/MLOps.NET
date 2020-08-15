using Azure.Storage.Blobs;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountModelRepository : IModelRepository
    {
        private readonly BlobContainerClient modelRepositoryClient;
        private readonly BlobContainerClient deploymentClient;
        private readonly IModelPathGenerator modelPathGenerator;

        public StorageAccountModelRepository(BlobContainerClient modelRepositoryClient, BlobContainerClient deploymentClient, IModelPathGenerator modelPathGenerator)
        {
            this.modelRepositoryClient = modelRepositoryClient;
            this.deploymentClient = deploymentClient;
            this.modelPathGenerator = modelPathGenerator;
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            var blobClient = this.modelRepositoryClient.GetBlobClient(this.modelPathGenerator.GetModelName(runId));

            using var fileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, true);
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            BlobClient blobClient = this.modelRepositoryClient.GetBlobClient(this.modelPathGenerator.GetModelName(runId));
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"No model exists for Run ID {runId}");
            }
            await blobClient.DownloadToAsync(destination);
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, Experiment experiment)
        {
            var deploymentPath = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, experiment.ExperimentName);

            var sourceModelBlob = this.modelRepositoryClient.GetBlobClient(this.modelPathGenerator.GetModelName(registeredModel.RunId));
            var deployedModelBlob = this.deploymentClient.GetBlobClient(deploymentPath);

            if (!sourceModelBlob.Exists())
            {
                throw new InvalidOperationException("The model to be deployed does not exist");
            }

            await deployedModelBlob.StartCopyFromUriAsync(sourceModelBlob.Uri);

            return deployedModelBlob.Uri.ToString();
        }

        public string GetDeploymentUri(Experiment experiment, DeploymentTarget deploymentTarget)
        {
            var deploymentPath = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, experiment.ExperimentName);

            return this.deploymentClient.GetBlobClient(deploymentPath).Uri.ToString();
        }
    }
}
