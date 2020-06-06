using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class LocalFileModelRepository : IModelRepository
    {
        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            var modelStoragePath = @"C:\MLOps";

            using (var fileStream = File.OpenRead(filePath))
            {
                string destFile = Path.Combine(modelStoragePath, $"{runId}.zip");
                await Task.Run(() => { File.Copy(filePath, destFile, true); });
            }
        }
    }
}
