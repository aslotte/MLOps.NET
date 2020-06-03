using MLOps.NET.SQLite.Entities;
using MLOps.NET.SQLite.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    public sealed class SQLiteMetaDataStore : IMetaDataStore
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
