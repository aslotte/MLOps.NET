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
    }
}
