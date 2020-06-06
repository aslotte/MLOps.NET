using System;
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
    }
}
