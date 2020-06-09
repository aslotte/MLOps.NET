using MLOps.NET.Entities.Entities;
using MLOps.NET.SQLite.Entities;
using MLOps.NET.SQLite.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class SQLiteMetaDataStore : IMetaDataStore
    {
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            using (var db = new LocalDbContext())
            {
                var experiment = new Experiment(name);
                await db.Experiments.AddAsync(experiment);
                await db.SaveChangesAsync();
                
                return experiment.Id;
            }
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            using (var db = new LocalDbContext())
            {
                var run = new Run(experimentId);
                await db.Runs.AddAsync(run);
                await db.SaveChangesAsync();
                
                return run.Id;
            }
        }

        ///<inheritdoc/>
        public IExperiment GetExperiment(string experimentName)
        {
            using (var db = new LocalDbContext())
            {
                return db.Experiments.Single(x => x.ExperimentName == experimentName);
            }
        }

        ///<inheritdoc/>
        public IEnumerable<IExperiment> GetExperiments()
        {
            using (var db = new LocalDbContext())
            {
                return db.Experiments;
            }
        }

        ///<inheritdoc/>
        public List<IMetric> GetMetrics(Guid runId)
        {
            using (var db = new LocalDbContext())
            {
                return db.Metrics.Where(x => x.RunId == runId).ToList<IMetric>();
            }
        }

        ///<inheritdoc/>
        public List<IRun> GetRuns(Guid experimentId)
        {
            using (var db = new LocalDbContext())
            {
                return db.Runs.Where(x => x.ExperimentId == experimentId).ToList<IRun>();
            }
        }

        public Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            throw new NotImplementedException();
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            using (var db = new LocalDbContext())
            {
                var metric = new Metric(runId, metricName, metricValue);
                await db.Metrics.AddAsync(metric);
                await db.SaveChangesAsync();
                
                return;
            }
        }
    }
}
