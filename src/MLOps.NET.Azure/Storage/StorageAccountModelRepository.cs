using Azure.Storage.Blobs;
using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountModelRepository : IModelRepository
    {
        private readonly BlobContainerClient blobContainerClient;
        private const string containerName = "model-repository";
        private const string fileExtension = ".zip";

        public StorageAccountModelRepository(string connectionString)
        {
            this.blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = this.blobContainerClient.GetBlobClient(runId.ToString() + fileExtension);
            using (var fileStream = File.OpenRead(filePath))
            {
                await blobClient.UploadAsync(fileStream, true);
            }
        }

        /// <summary>
        /// Downloads the model file from disk into the provided stream.
        /// </summary>
        /// <param name="runId">Run ID to download model for</param>
        /// <param name="destination">Destination stream to write model into</param>
        /// <returns>Task with result of download operation</returns>
        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = this.blobContainerClient.GetBlobClient($"{runId}{fileExtension}");
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"No model exists for Run ID {runId}");
            }
            await blobClient.DownloadToAsync(destination);
        }

        public string DeployModel(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            throw new NotImplementedException();
        }
    }
}
