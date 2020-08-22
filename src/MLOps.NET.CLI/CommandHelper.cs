using ConsoleTables;
using Dynamitey.DynamicObjects;
using mlops.Settings;
using MLOps.NET.Azure;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using MLOps.NET.SQLServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// Helper methods to execute commands from cli
    /// </summary>
    internal class CommandHelper
    {
        private IMLOpsContext mlOpsContext;
        private readonly SettingsHelper settingsHelper;

        public CommandHelper(SettingsHelper settingsHelper)
        {
            this.settingsHelper = settingsHelper;
            CreateMLOpsContext();
        }

        /// <summary>
        /// Update Data Source
        /// </summary>
        /// <param name="options"></param>
        internal void UpdateDataSource(SetDataSourceOptions options)
        {
            settingsHelper.UpdateDataSource(options);
        }

        private void CreateMLOpsContext()
        {
            var settings = settingsHelper.GetSettings();
            if (settings == null)
                return;

            switch(settings.DataSource)
            {
                case DataSource.CosmosDb:
                    mlOpsContext = new MLOpsBuilder()
                                   .UseLocalFileModelRepository()
                                   .UseCosmosDb(settings.CosmosDb.EndPoint, settings.CosmosDb.AccountKey)
                                   .Build();
                    break;

                case DataSource.SQLServer:
                    mlOpsContext = new MLOpsBuilder()
                                    .UseLocalFileModelRepository()
                                    .UseSQLServer(settings.SQLServer.ConnectionString)
                                    .Build();
                    break;
                case DataSource.SQLite:
                    mlOpsContext = new MLOpsBuilder()
                                    .UseLocalFileModelRepository()
                                    .UseSQLite()
                                    .Build();
                    break;
                default:
                    throw new Exception("Unsupported data source");
            }
        }

        /// <summary>
        /// Create Experiment
        /// </summary>
        /// <param name="options"></param>
        internal void CreateExperiment(CreateExperimentOptions options)
        {
            Console.WriteLine(mlOpsContext.LifeCycle.CreateExperimentAsync(options.ExperimentName).Result);
        }

        /// <summary>
        /// List Runs
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
            settingsHelper.SetCosmosConfiguration(options);
            CreateMLOpsContext();
        }

        /// <summary>
        /// List Run Artifacts
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
        /// List Metrics
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
