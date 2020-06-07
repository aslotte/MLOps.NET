using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class Metric : TableEntity, IMetric
    {
        public Metric() { }

        public Metric(Guid runId, string metricName, double value)
        {
            PartitionKey = runId.ToString();
            RowKey = MetricName = metricName;
            Value = value;
            RunId = runId;
        }

        public string MetricName { get; set; }

        public double Value { get; set; }

        public Guid RunId { get; set; }
    }
}
