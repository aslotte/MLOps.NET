using MLOps.NET.Entities;
using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET
{
    public class MLLifeCycleManager
    {
        private readonly IMetaDataStore metaDataStore;

        public MLLifeCycleManager(string connectionString)
        {
            this.metaDataStore = new StorageAccountMetaDataStore(connectionString);
        }

        public async Task<Guid> CreateExperimentAsync(string name)
        {
            var newExperiment = new Experiment(name);

            var insertedExperiment = await this.metaDataStore.CreateExperiementAsync(newExperiment);
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
