using System;

namespace MLOps.NET.Entities.Interfaces
{
    /// <summary>
    /// Metric associated with a given training run
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        /// Name of metric, e.g. Accurarcy, F1Score or Precision/Recall
        /// </summary>
        string MetricName { get; set; }
        /// <summary>
        /// Metric value
        /// </summary>
        double Value { get; set; }
        /// <summary>
        /// Unique RunId
        /// </summary>
        Guid RunId { get; set; }
    }
}