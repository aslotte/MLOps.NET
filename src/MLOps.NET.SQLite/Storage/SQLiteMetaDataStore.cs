﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<Dictionary<IRun, IEnumerable<IMetric>>> GetAllRunsAndMetricsByExperimentIdAsync(Guid experimentId)
        {
            using (var db = new LocalDbContext())
            {
                var allRuns = await db.Runs.Where(r => r.ExperimentId == experimentId).ToListAsync();
                var allMetrics = new Dictionary<IRun, IEnumerable<IMetric>>();

                foreach (var run in allRuns)
                {
                    var metrics = await db.Metrics.Where(m => m.RunId == run.Id).ToListAsync();
                    allMetrics.Add(run, metrics);
                }
                return allMetrics;
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
