using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET
{
    public class MLLifeCycleManager : IMLLifeCycleManager
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

        public async Task<Guid> CreateRunAsync(string experimentName)
        {
            EnsureStorageProviderConfigured();

            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId);
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            EnsureStorageProviderConfigured();

            await MetaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        public async Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class
        {
            var metricsType = metrics.GetType();

            var properties = metricsType.GetProperties().Where(x => x.PropertyType == typeof(double));

            foreach (var metric in properties)
            {
                await LogMetricAsync(runId, metric.Name, (double)metric.GetValue(metrics));
            }
        }

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

        public async Task<Dictionary<IRun, IEnumerable<IMetric>>> GetAllRunsAndMetricsByExperimentIdAsync(Guid experimentId)
        {
            return await MetaDataStore.GetAllRunsAndMetricsByExperimentIdAsync(experimentId);
        }

        public async Task<(IRun Run,IEnumerable<IMetric> Metrics)> GetBestRun(Guid experimentId, string metricName)
        {
            EnsureStorageProviderConfigured();
            var allMetricsForAnExperiment = await MetaDataStore.GetAllRunsAndMetricsByExperimentIdAsync(experimentId);
            // Flattening the metrics for all the runs for a given experiment into one list and finding the best among them.
            var bestRunId = allMetricsForAnExperiment.Values.SelectMany(m => m)
                .Where(m => m.MetricName == metricName)
                .OrderByDescending(m => m.Value)
                .First().RunId;
            var bestRun = allMetricsForAnExperiment.Keys.Where(r => r.Id == bestRunId).First();
            return (bestRun, allMetricsForAnExperiment[bestRun]);
        }
    }
}
