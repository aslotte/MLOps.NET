using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Service to store run artifacts
    /// </summary>
    public interface IModelRepository
    {
        /// <summary>
        /// Uploads a machine learning model to the model repository
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="filePath">Relative or absolute filepath</param>
        /// <returns></returns>
        Task UploadModelAsync(Guid runId, string filePath);

        /// <summary>
        /// Downloads a machine learning model from the model repository
        /// </summary>
        /// <param name="runId">Run ID to download model for</param>
        /// <param name="destination">Destination stream to write downloaded model to</param>
        /// <returns>Stream with model contents</returns>
        Task DownloadModelAsync(Guid runId, Stream destination);
    }
}
