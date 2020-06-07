using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET
{
    /// <summary>
    /// Access point to manage the lifecycle of a machin learning model
    /// </summary>
    public interface IModelContext
    {
        /// <summary>
        /// Creates a unique experiment based on a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Experiment Id</returns>
        Task<Guid> CreateExperimentAsync(string name);
        /// <summary>
        /// Creates a unique run for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns>Run Id</returns>
        Task<Guid> CreateRunAsync(Guid experimentId);
        /// <summary>
        /// Creates a unique run and experiment given an experiment name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns>Run Id</returns>
        Task<Guid> CreateRunAsync(string experimentName);
        /// <summary>
        /// Logs a given metric for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        Task LogMetricAsync(Guid runId, string metricName, double metricValue);
        /// <summary>
        /// Logs all evaluation metrics of type double for a machine learning model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runId"></param>
        /// <param name="metrics"></param>
        /// <returns></returns>
        Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class;
        /// <summary>
        /// Uploads the model artifact of a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="filePath">Absolute or relative path to the model</param>
        /// <returns></returns>
        Task UploadModelAsync(Guid runId, string filePath);
    }
}
