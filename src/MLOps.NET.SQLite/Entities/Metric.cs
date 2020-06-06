using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class Metric : IMetric
    {
        public Metric(Guid runId, string metricName, double value)
        {
            MetricName = metricName;
            Value = value;
            RunId = runId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string MetricName { get; set; }
        public double Value { get; set; }
        public Guid RunId { get; set; }
    }
}
