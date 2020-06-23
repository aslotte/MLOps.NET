﻿using MLOps.NET.Entities;
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
        /// <param name="gitCommitHash">Optional, sets the linked git commit hash</param>
        /// <returns></returns>
        Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "");
        
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
        /// Get run by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        IRun GetRun(Guid runId);

        /// <summary>
        /// Saves the confusion matrix as a json serialized string
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="confusionMatrix"></param>
        /// <returns></returns>
        Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix);

        /// <summary>
        /// Get all metrics by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        List<IMetric> GetMetrics(Guid runId);

        /// <summary>
        /// Logs a given hyperparameter for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task LogHyperParameterAsync(Guid runId, string name, string value);

        /// <summary>
        /// Sets the training time for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan);

        /// <summary>
        /// Gets the confusion matrix for a run.
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        ConfusionMatrix GetConfusionMatrix(Guid runId);
    }
}
