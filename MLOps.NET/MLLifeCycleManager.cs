using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET
{
    public class MLLifeCycleManager
    {
        public IMetaDataStore MetaDataStore { get; set; }

        public IModelRepository ModelRepository { get; set; }

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

        /// <summary>
        /// Uploads the model zip file to azure blob storage model-repository container.
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            EnsureStorageProviderConfigured();
            await ModelRepository.UploadModelAsync(runId, filePath);
        }

        private void EnsureStorageProviderConfigured()
        {
            if (MetaDataStore == null || ModelRepository == null)
            {
                throw new InvalidOperationException("The storage provider has not been properly set up. Please call Rutix, Daniel and usertyuu should you have any questions");
            }
        }
    }
}
