using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET
{
    ///<inheritdoc cref="IMLLifeCycleManager"/>
    public class MLLifeCycleManager : IMLLifeCycleManager
    {
        ///<inheritdoc/>
        public IMetaDataStore MetaDataStore { get; set; }

        ///<inheritdoc/>
        public IModelRepository ModelRepository { get; set; }

        ///<inheritdoc/>
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            EnsureStorageProviderConfigured();

            return await MetaDataStore.CreateExperimentAsync(name);
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            EnsureStorageProviderConfigured();

            return await MetaDataStore.CreateRunAsync(experimentId);
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateRunAsync(string experimentName)
        {
            EnsureStorageProviderConfigured();

            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId);
        }

        ///<inheritdoc/>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            EnsureStorageProviderConfigured();

            await MetaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        ///<inheritdoc/>
        public async Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class
        {
            var metricsType = metrics.GetType();

            var properties = metricsType.GetProperties().Where(x => x.PropertyType == typeof(double));

            foreach (var metric in properties)
            {
                await LogMetricAsync(runId, metric.Name, (double)metric.GetValue(metrics));
            }
        }

        ///<inheritdoc/>
        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            EnsureStorageProviderConfigured();
            await ModelRepository.UploadModelAsync(runId, filePath);
        }

        ///<inheritdoc/>
        private void EnsureStorageProviderConfigured()
        {
            if (MetaDataStore == null || ModelRepository == null)
            {
                throw new InvalidOperationException("The storage provider has not been properly set up. Please call Rutix, Daniel and usertyuu should you have any questions");
            }
        }
    }
}
