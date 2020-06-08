using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class LocalFileModelRepository : IModelRepository
    {
        public readonly string destinationFolder;

        public LocalFileModelRepository(string destinationFolder = @"C:\MLOps")
        {
            this.destinationFolder = destinationFolder;
        }

        /// <summary>
        /// Upload model file from source file path to destination folder. The file name is runId (guid) with .zip extension.
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public async Task UploadModelAsync(Guid runId, string sourceFilePath)
        {
            using (var fileStream = File.OpenRead(sourceFilePath))
            {
                string destFile = Path.Combine(destinationFolder, $"{runId}.zip");
                await Task.Run(() => { File.Copy(sourceFilePath, destFile, true); });
            }
        }
    }
}
