using System;

namespace MLOps.NET.Entities.Entities
{
    public interface IMetric
    {
        string MetricName { get; set; }
        double Value { get; set; }
        Guid RunId { get; set; }
    }
}