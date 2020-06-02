using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET
{
    public class MLLifeCycleManager
    {
        public IMetaDataStore MetaDataStore { get; set; }

        public async Task<Guid> CreateExperimentAsync(string name)
        {
            EnsureStorageProviderConfigured();

            return await MetaDataStore.CreateExperimentAsync(name);
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            EnsureStorageProviderConfigured();

            return await MetaDataStore.CreateRunAsync(experimentId);
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            EnsureStorageProviderConfigured();

            await MetaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        private void EnsureStorageProviderConfigured()
        {
            if (MetaDataStore == null)
            {
                throw new InvalidOperationException("The storage provider has not been properly set up. Please call Rutix, Daniel and usertyuu should you have any questions");
            }
        }
    }
}
