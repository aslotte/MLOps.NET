using ConsoleTables;
using Dynamitey.DynamicObjects;
using mlops.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    internal class CommandHelper
    {
        private readonly IMLOpsContext mlOpsContext;

        public CommandHelper()
        {
            mlOpsContext = new MLOpsBuilder().UseSQLite().UseLocalFileModelRepository().Build();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal void ListRuns(ListRunsOptions options)
        {
            var experiment = mlOpsContext.LifeCycle.GetExperiment(options.ExperimentName);
            ConsoleTable.From(experiment.Runs.Select(r => new
            {
                RunId = r.RunId,
                Date = r.RunDate
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        internal void SetCosmosConfiguration(ConfigCosmosOptions options)
        {
            var settings = new Settings();
            settings.SetCosmosConfiguration(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal void ListRunArtifacts(ListRunArtifactsOptions options)
        {
            var runArtifacts = mlOpsContext.LifeCycle.GetRun(options.RunId).RunArtifacts;
            ConsoleTable.From(runArtifacts.Select(r => new
            {
                RunArtifactId = r.RunArtifactId,
                Date = r.Name
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal void ListMetrics(ListMetricsOptions options)
        {
            var metrics = mlOpsContext.LifeCycle.GetRun(options.RunId).Metrics;
            ConsoleTable.From(metrics.Select(m => new
            {
                MetricName = m.MetricName,
                Value = m.Value
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }
    }
}
