using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Metric associated with a given training run
    /// </summary>
    public sealed class Metric
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="value"></param>
        public Metric(Guid runId, string metricName, double value)
        {
            MetricId = Guid.NewGuid();
            MetricName = metricName;
            Value = value;
            RunId = runId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid MetricId { get; set; }

        /// <summary>
        /// Name of metric, e.g. Accurarcy, F1Score or Precision/Recall
        /// </summary>
        public string MetricName { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Run
        /// </summary>
        public Run Run { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }
    }
}
