using MLOps.NET.Entities.Entities;
using MLOps.NET.SQLServer.Entities;
using MLOps.NET.SQLServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class SQLServerMetaDataStore : IMetaDataStore
    {
        private readonly IDbContextFactory contextFactory;

        public SQLServerMetaDataStore(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<Guid> CreateExperimentAsync(string name)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var experiment = new Experiment(name);
                await db.Experiments.AddAsync(experiment);
                await db.SaveChangesAsync();

                return experiment.Id;
            }
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var run = new Run(experimentId)
                {
                    GitCommitHash = gitCommitHash
                };

                await db.Runs.AddAsync(run);
                await db.SaveChangesAsync();

                return run.Id;
            }
        }

        ///<inheritdoc/>
        public IExperiment GetExperiment(string experimentName)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Experiments.Single(x => x.ExperimentName == experimentName);
            }
        }

        ///<inheritdoc/>
        public IEnumerable<IExperiment> GetExperiments()
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Experiments;
            }
        }

        ///<inheritdoc/>
        public List<IMetric> GetMetrics(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Metrics.Where(x => x.RunId == runId).ToList<IMetric>();
            }
        }

        public IRun GetRun(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Runs.FirstOrDefault(x => x.Id == runId);
            }
        }

        ///<inheritdoc/>
        public List<IRun> GetRuns(Guid experimentId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Runs.Where(x => x.ExperimentId == experimentId).ToList<IRun>();
            }
        }

        ///<inheritdoc/>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var hyperParameter = new HyperParameter(runId, name, value);
                await db.HyperParameters.AddAsync(hyperParameter);
                await db.SaveChangesAsync();

                return;
            }
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var metric = new Metric(runId, metricName, metricValue);
                await db.Metrics.AddAsync(metric);
                await db.SaveChangesAsync();

                return;
            }
        }

        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var existingRun = db.Runs.FirstOrDefault(x => x.Id == runId);
                if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

                existingRun.TrainingTime = timeSpan;

                await db.SaveChangesAsync();
            }
        }
    }
}
