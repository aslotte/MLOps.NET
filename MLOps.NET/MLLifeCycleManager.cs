using MLOps.NET.Entities;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
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

            var insertedExperiment = await this.metaDataStore.CreateExperimentAsync(newExperiment);
            return insertedExperiment.Id;
        }

        public async Task<Guid> CreateRun(Guid experimentId)
        {
            var run = new Run(experimentId);

            var insertedRun = await this.metaDataStore.CreateRunAsync(run);

            return insertedRun.Id;
        }

        public void LogMetrics(Dictionary<string, string> metrics)
        {

        }

        public void LogModel()
        {

        }
    }
}
