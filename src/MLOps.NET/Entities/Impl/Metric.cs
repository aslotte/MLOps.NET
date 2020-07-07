using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IMetric"/>
    public sealed class Metric : IMetric
    {
        ///<inheritdoc cref="IMetric"/>
        public Metric(Guid runId, string metricName, double value)
        {
            MetricName = metricName;
            Value = value;
            RunId = runId;
            Id = Guid.NewGuid();
        }

        ///<inheritdoc cref="IMetric"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IMetric"/>
        public string MetricName { get; set; }

        ///<inheritdoc cref="IMetric"/>
        public double Value { get; set; }

        ///<inheritdoc cref="IMetric"/>
        public Guid RunId { get; set; }
    }
}
