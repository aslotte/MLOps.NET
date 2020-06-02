using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountMetaDataStore : IMetaDataStore
    {
        public Task<Guid> CreateExperimentAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateRunAsync(Guid experimentId)
        {
            throw new NotImplementedException();
        }

        public Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            throw new NotImplementedException();
        }
    }
}
