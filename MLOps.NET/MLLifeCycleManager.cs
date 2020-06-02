using MLOps.NET.Entities;
using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET
{
    public class MLLifeCycleManager
    {
        // No readonly as a readonly field can only be set in a constructor.
        private IMetaDataStore metaDataStore;

        /// <summary>
        /// Ensures azure storage account is created from the connection string.
        /// </summary>
        /// <param name="connectionString">azure storage account connection string</param>
        /// <returns></returns>
        public MLLifeCycleManager UseAzureStorage(string connectionString)
        {
            this.metaDataStore = new StorageAccountMetaDataStore(connectionString);
            return this;
        }

        public async Task<Guid> CreateExperimentAsync(string name)
        {
            var newExperiment = new Experiment(name);

            var insertedExperiment = await this.metaDataStore.CreateExperimentAsync(newExperiment);
            return insertedExperiment.Id;
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            var run = new Run(experimentId);

            var insertedRun = await this.metaDataStore.CreateRunAsync(run);

            return insertedRun.Id;
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            var metric = new Metric(runId, metricName, metricValue);

            await this.metaDataStore.LogMetricAsync(metric);
        }

        public void LogModel()
        {

        }
    }
}
