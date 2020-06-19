using Microsoft.ML.Data;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Entities.Interfaces;
using MLOps.NET.SQLite.Entities;
using MLOps.NET.SQLite.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfusionMatrix = MLOps.NET.SQLite.Entities.ConfusionMatrix;

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
        public IConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            using (var db = new LocalDbContext())
            {
                return db.ConfusionMatrices.Single(x => x.RunId == runId);
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

        public IRun GetRun(Guid runId)
        {
            using (var db = new LocalDbContext())
            {
                return db.Runs.FirstOrDefault(x => x.Id == runId);
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

        public async Task LogConfusionMatrixAsync(Guid runId, Microsoft.ML.Data.ConfusionMatrix confusionMatrix)
        {
            using (var db = new LocalDbContext())
            {
                var conMatrix = new ConfusionMatrix(runId, confusionMatrix.NumberOfClasses,
                                                confusionMatrix.PerClassPrecision.ToList(),
                                                confusionMatrix.PerClassRecall.ToList(),
                                                confusionMatrix.Counts);
                conMatrix.SerializedDetails = JsonConvert.SerializeObject(conMatrix);
                await db.ConfusionMatrices.AddAsync(conMatrix);
                await db.SaveChangesAsync();
                return;
            }
        }

        ///<inheritdoc/>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            using (var db = new LocalDbContext())
            {
                var hyperParameter = new HyperParameter(runId, name, value);
                await db.HyperParameters.AddAsync(hyperParameter);
                await db.SaveChangesAsync();

                return;
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

        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            using (var db = new LocalDbContext())
            {
                var existingRun = db.Runs.FirstOrDefault(x => x.Id == runId);
                if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

                existingRun.TrainingTime = timeSpan;

                await db.SaveChangesAsync();
            }
        }
    }
}
