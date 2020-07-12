using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage metrics
    /// </summary>
    public interface IMetricRepository
    {
        /// <summary>
        /// Logs a given metric for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        Task LogMetricAsync(Guid runId, string metricName, double metricValue);

        /// <summary>
        /// Get all metrics by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        List<Metric> GetMetrics(Guid runId);
    }
}
