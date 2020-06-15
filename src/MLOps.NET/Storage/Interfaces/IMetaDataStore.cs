using Microsoft.ML.Data;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Get all runs 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IExperiment> GetExperiments();
        /// <summary>
        /// Get specific experiment by name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns></returns>
        IExperiment GetExperiment(string experimentName);
        /// <summary>
        /// Get all runs by experiment id
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        List<IRun> GetRuns(Guid experimentId);
        /// <summary>
        /// Get all metrics by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        List<IMetric> GetMetrics(Guid runId);

        /// <summary>
        /// Logs the confusion matrix string representaion in the store
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="value">string representaion of the confusion matrix</param>
        /// <returns></returns>
        Task LogConfusionMatrixAsync(Guid runId, string value);

        /// <summary>
        /// Logs a given hyperparameter for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task LogHyperParameterAsync(Guid runId, string name, string value);
    }
}
