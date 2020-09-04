using ConsoleTables;
using MLOps.NET.AWS;
using MLOps.NET.Azure;
using MLOps.NET.CLI.Settings;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using MLOps.NET.SQLServer;
using System;
using System.Linq;

namespace MLOps.NET.CLI
{
    internal class LifeCycleCatalogCliHelper : MLOpsBuilderCliBase
    {

        public LifeCycleCatalogCliHelper(CliSettingsWriter settingsHelper) :base(settingsHelper)
        {
            
        }

        internal void CreateRun(CreateRunOptions options)
        {
            CreateMLOpsContextIfNotExists();

            var run = mlOpsContext.LifeCycle.CreateRunAsync(options.ExperimentId).Result;
            Console.WriteLine($"Created Run {run.RunId}");
        }

        internal void CreateExperiment(CreateExperimentOptions options)
        {
            CreateMLOpsContextIfNotExists();

            var experimentId = mlOpsContext.LifeCycle.CreateExperimentAsync(options.ExperimentName).Result;
            Console.WriteLine($"Created experiment {experimentId}");
        }

        internal void ListRuns(ListRunsOptions options)
        {
            CreateMLOpsContextIfNotExists();

            var experiment = mlOpsContext.LifeCycle.GetExperiment(options.ExperimentName);
            ConsoleTable.From(experiment.Runs.Select(r => new
            {
                r.RunId,
                Date = r.RunDate
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        internal void ListRunArtifacts(ListRunArtifactsOptions options)
        {
            CreateMLOpsContextIfNotExists();
            var runArtifacts = mlOpsContext.LifeCycle.GetRun(options.RunId).RunArtifacts;
            ConsoleTable.From(runArtifacts.Select(r => new
            {
                r.RunArtifactId,
                Date = r.Name
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        internal void ListMetrics(ListMetricsOptions options)
        {
            CreateMLOpsContextIfNotExists();
            var metrics = mlOpsContext.LifeCycle.GetRun(options.RunId).Metrics;
            ConsoleTable.From(metrics.Select(m => new
            {
                m.MetricName,
                m.Value
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }
    }
}
