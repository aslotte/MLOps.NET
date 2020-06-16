using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class LocalFileModelRepository : IModelRepository
    {
        public readonly string destinationFolder;
        private readonly IFileSystem fileSystem;

        public LocalFileModelRepository(IFileSystem fileSystem, string destinationFolder = null)
        {
            if(string.IsNullOrWhiteSpace(destinationFolder))
                destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");

            this.destinationFolder = destinationFolder;
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Upload model file from source file path to destination folder. The file name is runId (guid) with .zip extension.
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public async Task UploadModelAsync(Guid runId, string sourceFilePath)
        {
            using (var fileStream = this.fileSystem.File.OpenRead(sourceFilePath))
            {
                if (!this.fileSystem.Directory.Exists(destinationFolder))
                    this.fileSystem.Directory.CreateDirectory(destinationFolder);

                string destFile = this.fileSystem.Path.Combine(destinationFolder, $"{runId}.zip");
                await Task.Run(() => { this.fileSystem.File.Copy(sourceFilePath, destFile, true); });
            }
        }

        /// <summary>
        /// Reads the model file from disk into the provided stream.
        /// </summary>
        /// <param name="runId">The run ID to load model data for</param>
        /// <param name="destination">Destination stream to read model into</param>
        /// <returns>Task with result of download operation</returns>
        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            string sourceFile = this.fileSystem.Path.Combine(destinationFolder, $"{runId}.zip");
            if (!this.fileSystem.File.Exists(sourceFile))
            {
                throw new FileNotFoundException($"Run artifact {sourceFile} was not found");
            }
            using (var fileStream = this.fileSystem.File.OpenRead(sourceFile))
            {
                await fileStream.CopyToAsync(destination);
            }
        }
    }
}
