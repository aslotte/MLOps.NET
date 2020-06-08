using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET
{
    ///<inheritdoc cref="IMLLifeCycleManager"/>
    public class MLLifeCycleManager : IMLLifeCycleManager
    {
        /// <summary>
        /// Repository for model metadata 
        /// </summary>
        public IMetaDataStore MetaDataStore { get; set; }

        /// <summary>
        /// Repository for run artifacts such as models
        /// </summary>
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
        public IRun GetBestRun(Guid experimentId, string metricName)
        {
            EnsureStorageProviderConfigured();
            var allRuns = MetaDataStore.GetRuns(experimentId);
            var bestRunId = allRuns.SelectMany(r => r.Metrics)
                .Where(m => m.MetricName.ToLowerInvariant() == metricName.ToLowerInvariant())
                .OrderByDescending(m => m.Value)
                .First().RunId;

            return allRuns.FirstOrDefault(r => r.Id == bestRunId);
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
