﻿using MLOps.NET.Entities;
using MLOps.NET.Entities.Entities;
using MLOps.NET.SQLite.Entities;
using MLOps.NET.SQLite.Storage;
using Newtonsoft.Json;
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

        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            using (var db = new LocalDbContext())
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

        public ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            using (var db = new LocalDbContext())
            {
                var confusionMatrixEntity = db.ConfusionMatrices.SingleOrDefault(x => x.RunId == runId);         
                if (confusionMatrixEntity == null) return null;
                return JsonConvert.DeserializeObject<ConfusionMatrix>(confusionMatrixEntity.SerializedMatrix);
            }
        }

        public IExperiment GetExperiment(string experimentName)
        {
            using (var db = new LocalDbContext())
            {
                return db.Experiments.Single(x => x.ExperimentName == experimentName);
            }
        }

        public IEnumerable<IExperiment> GetExperiments()
        {
            using (var db = new LocalDbContext())
            {
                return db.Experiments;
            }
        }

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

        public List<IRun> GetRuns(Guid experimentId)
        {
            using (var db = new LocalDbContext())
            {
                return db.Runs.Where(x => x.ExperimentId == experimentId).ToList<IRun>();
            }
        }

        public async Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            using (var db = new LocalDbContext())
            {
                var conMatrix = new ConfusionMatrixEntity(runId);
                conMatrix.SerializedMatrix = JsonConvert.SerializeObject(confusionMatrix);
                await db.ConfusionMatrices.AddAsync(conMatrix);
                await db.SaveChangesAsync();
            }
        }

        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            using (var db = new LocalDbContext())
            {
                var hyperParameter = new HyperParameter(runId, name, value);
                await db.HyperParameters.AddAsync(hyperParameter);
                await db.SaveChangesAsync();
            }
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            using (var db = new LocalDbContext())
            {
                var metric = new Metric(runId, metricName, metricValue);
                await db.Metrics.AddAsync(metric);
                await db.SaveChangesAsync();
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
