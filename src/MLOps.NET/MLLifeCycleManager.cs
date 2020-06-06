using MLOps.NET.Storage;
using System;
using System.Linq;
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

        /// <summary>
        /// Creates a run and an experiment in one operation
        /// </summary>
        /// <param name="experimentName">Unique name of experiment</param>
        /// <returns>RunId</returns>
        public async Task<Guid> CreateRunAsync(string experimentName)
        {
            EnsureStorageProviderConfigured();

            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId);
        }

        /// <summary>
        /// Log given metric
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            EnsureStorageProviderConfigured();

            await MetaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        /// <summary>
        /// Logs all metrics of type double on provided input
        /// </summary>
        /// <typeparam name="T">Type of evaluation metric</typeparam>
        /// <param name="runId"></param>
        /// <param name="metrics">Evaluation metrics</param>
        /// <returns></returns>
        public async Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class
        {
            var metricsType = metrics.GetType();

            var properties = metricsType.GetProperties().Where(x => x.PropertyType == typeof(double));

            foreach (var metric in properties)
            {
                await LogMetricAsync(runId, metric.Name, (double)metric.GetValue(metrics));
            }
        }

        /// <summary>
        /// Uploads the model zip artifact to the desired storage location.
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="filePath">local file path where the model file is saved</param>
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
