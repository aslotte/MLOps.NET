using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountModelRepository : IModelRepository
    {
        private readonly BlobContainerClient modelRepositoryClient;
        private readonly BlobContainerClient deploymentClient;
        private const string fileExtension = ".zip";

        public StorageAccountModelRepository(BlobContainerClient modelRepositoryClient, BlobContainerClient deploymentClient)
        {
            this.modelRepositoryClient = modelRepositoryClient;
            this.deploymentClient = deploymentClient;
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            BlobClient blobClient = this.modelRepositoryClient.GetBlobClient(GetModelName(runId));

            using var fileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, true);
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            BlobClient blobClient = this.modelRepositoryClient.GetBlobClient(GetModelName(runId));
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"No model exists for Run ID {runId}");
            }
            await blobClient.DownloadToAsync(destination);
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            var experimentName = registeredModel.Experiment.ExperimentName;
            var deploymentBlob = string.Join("/", experimentName, deploymentTarget.Name, $"{registeredModel.RunId}{fileExtension}");

            var sourceModelBlob = this.modelRepositoryClient.GetBlobClient(GetModelName(registeredModel.RunId));

            if (!sourceModelBlob.Exists())
            {
                throw new InvalidOperationException("The model to be deployed does not exist");
            }

            var deployedModelBlob = this.deploymentClient.GetBlobClient(deploymentBlob);
            await deployedModelBlob.StartCopyFromUriAsync(sourceModelBlob.Uri);

            return deployedModelBlob.Uri.ToString();
        }

        private string GetModelName(Guid runId) => $"{runId}{fileExtension}";
    }
}
