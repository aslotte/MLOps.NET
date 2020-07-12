using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
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
        private readonly IExperimentRepository experimentRepository;
        private readonly IRunRepository runRepository;
        private readonly IClock clock;

        /// <summary>
        /// ctor       
        /// </summary>
        /// <param name="experimentRepository"></param>
        /// <param name="runRepository"></param>
        /// <param name="clock">Abstraction of DateTime</param>
        public LifeCycleCatalog(IExperimentRepository experimentRepository, IRunRepository runRepository, IClock clock)
        {
            this.experimentRepository = experimentRepository;
            this.runRepository = runRepository;
            this.clock = clock;
        }

        /// <summary>
        /// Creates a unique experiment based on a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Experiment Id</returns>
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            return await experimentRepository.CreateExperimentAsync(name);
        }

        /// <summary>
        /// Creates a unique run for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="gitCommitHash">Optional, set the linked git commit hash</param>
        /// <returns>Run Id</returns>
        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            return await runRepository.CreateRunAsync(experimentId, gitCommitHash);
        }

        /// <summary>
        /// Creates a unique run and experiment given an experiment name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="gitCommitHash">Optional, set the linked git commit hash</param>
        /// <returns>Run Id</returns>
        public async Task<Guid> CreateRunAsync(string experimentName, string gitCommitHash = "")
        {
            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId, gitCommitHash);
        }

        /// <summary>
        /// Gets experiment by name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns></returns>
        public Experiment GetExperiment(string experimentName)
        {
            return this.experimentRepository.GetExperiment(experimentName);
        }

        /// <summary>
        /// Get a run by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public Run GetRun(Guid runId)
        {
            return this.runRepository.GetRun(runId);
        }

        /// <summary>
        /// Get a run by commit hash
        /// </summary>
        /// <param name="commitHash"></param>
        /// <returns></returns>
        public Run GetRun(string commitHash)
        {
            return this.runRepository.GetRun(commitHash);
        }

        /// <summary>
        /// Gets the best run for an experiment based on a metric for e.g "Accuracy"
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="metricName"></param>
        /// <returns></returns>
        public Run GetBestRun(Guid experimentId, string metricName)
        {
            var allRuns = runRepository.GetRuns(experimentId);
            var bestRunId = allRuns.SelectMany(r => r.Metrics)
                .Where(m => m.MetricName.ToLowerInvariant() == metricName.ToLowerInvariant())
                .OrderByDescending(m => m.Value)
                .First().RunId;

            return allRuns.FirstOrDefault(r => r.RunId == bestRunId);
        }

        /// <summary>
        /// Sets the training time for a run calculated as current time minus run start time 
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public async Task SetTrainingTimeAsync(Guid runId)
        {
            var endTime = this.clock.UtcNow;
            var run = runRepository.GetRun(runId);

            var trainingTime = endTime.Subtract(run.RunDate);

            await SetTrainingTimeAsync(runId, trainingTime);
        }

        /// <summary>
        /// Sets the training time for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="timeSpan">Training time</param>
        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            await runRepository.SetTrainingTimeAsync(runId, timeSpan);
        }
    }
}
