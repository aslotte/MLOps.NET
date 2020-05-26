using Microsoft.Azure.Cosmos.Table;
using System;

namespace MLOps.NET.Entities
{
    internal sealed class Metric : TableEntity
    {
        public Metric(Guid runId, string metricName, double value)
        {
            PartitionKey = runId.ToString();
            RowKey = MetricName = metricName;
            Value = value;
        }

        public string MetricName { get; set; }
        public double Value { get; set; }
    }
}
