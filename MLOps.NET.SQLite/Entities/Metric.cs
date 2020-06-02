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
        }

        public string MetricName { get; set; }
        public double Value { get; set; }
    }
}
