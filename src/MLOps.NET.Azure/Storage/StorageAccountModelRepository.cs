using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading;
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
    }
}
