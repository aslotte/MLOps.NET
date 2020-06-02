using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    public interface IMetaDataStore
    {
        Task<Guid> CreateExperimentAsync(string name);

        Task<Guid> CreateRunAsync(Guid experimentId);

        Task LogMetricAsync(Guid runId, string metricName, double metricValue);
    }
}
