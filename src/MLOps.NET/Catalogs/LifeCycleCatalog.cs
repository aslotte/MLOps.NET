using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Exposes experiment related operations
    /// </summary>
    public sealed class LifeCycleCatalog
    {
        private readonly IMetaDataStore metaDataStore;

        /// <summary>
        /// ctor       
        /// </summary>
        /// <param name="metaDataStore"></param>
        public LifeCycleCatalog(IMetaDataStore metaDataStore)
        {
            this.metaDataStore = metaDataStore;
        }

        /// <summary>
        /// Creates a unique experiment based on a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Experiment Id</returns>
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            return await metaDataStore.CreateExperimentAsync(name);
        }

        /// <summary>
        /// Creates a unique run for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns>Run Id</returns>
        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            return await metaDataStore.CreateRunAsync(experimentId);
        }

        /// <summary>
        /// Creates a unique run and experiment given an experiment name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns>Run Id</returns>
        public async Task<Guid> CreateRunAsync(string experimentName)
        {
            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId);
        }

        /// <summary>
        /// Gets the best run for an experiment based on a metric for e.g "Accuracy"
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="metricName"></param>
        /// <returns></returns>
        public IRun GetBestRun(Guid experimentId, string metricName)
        {
            var allRuns = metaDataStore.GetRuns(experimentId);
            var bestRunId = allRuns.SelectMany(r => r.Metrics)
                .Where(m => m.MetricName.ToLowerInvariant() == metricName.ToLowerInvariant())
                .OrderByDescending(m => m.Value)
                .First().RunId;

            return allRuns.FirstOrDefault(r => r.Id == bestRunId);
        }
    }
}
