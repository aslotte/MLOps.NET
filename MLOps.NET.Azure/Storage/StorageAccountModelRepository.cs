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
        private const string _containerName = "model-repository";
        private const string _fileExtension = ".zip";

        public StorageAccountModelRepository(string connectionString)
        {
            this.blobContainerClient = new BlobContainerClient(connectionString, _containerName);
        }

        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = this.blobContainerClient.GetBlobClient(runId.ToString() + _fileExtension);
            using (var fileStream = File.OpenRead(filePath))
            {
                await blobClient.UploadAsync(fileStream, true);
            }
        }
    }
}
