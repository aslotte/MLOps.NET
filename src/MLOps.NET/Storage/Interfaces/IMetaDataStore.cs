using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Service to access metadata of a given machine learning model
    /// </summary>
    public interface IMetaDataStore
    {
        /// <summary>
        /// Creates a unqiue experiment with provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Guid> CreateExperimentAsync(string name);
        /// <summary>
        /// Creates a unqiue run for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        Task<Guid> CreateRunAsync(Guid experimentId);
        /// <summary>
        /// Logs a given metric for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        Task LogMetricAsync(Guid runId, string metricName, double metricValue);
    }
}
