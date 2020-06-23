using MLOps.NET.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to a model
    /// </summary>
    public sealed class ModelCatalog
    {
        private readonly IModelRepository modelRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="modelRepository"></param>
        public ModelCatalog(IModelRepository modelRepository)
        {
            this.modelRepository = modelRepository;
        }

        ///<inheritdoc/>
        public async Task UploadAsync(Guid runId, string filePath)
        {
            await modelRepository.UploadModelAsync(runId, filePath);
        }

        /// <summary>
        /// Downloads the model data for the provided run ID into the stream
        /// </summary>
        /// <param name="runId">Run ID to download model data for</param>
        /// <param name="stream">Stream to populate with loaded data</param>
        /// <returns>Task containing result of operation</returns>
        public Task DownloadAsync(Guid runId, Stream stream)
        {
            return modelRepository.DownloadModelAsync(runId, stream);
        }
    }
}
