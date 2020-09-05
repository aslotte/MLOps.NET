using ConsoleTables;
using MLOps.NET.CLI.Commands.Interfaces;
using System;
using System.Linq;

namespace MLOps.NET.CLI
{
    internal sealed class LifeCycleCatalogCli : ILifeCycleCatalogCli
    {
        private readonly IMLOpsBuilderCli mlOpsBuilderCli;

        public LifeCycleCatalogCli(IMLOpsBuilderCli mlOpsBuilderCli)
        {
            this.mlOpsBuilderCli = mlOpsBuilderCli;
        }

        public void CreateRun(CreateRunOptions options)
        {
            var mlOpsContext = mlOpsBuilderCli.CreateMLOpsContext();

            var run = mlOpsContext.LifeCycle.CreateRunAsync(options.ExperimentId).Result;
            Console.WriteLine($"Created Run {run.RunId}");
        }

        public void CreateExperiment(CreateExperimentOptions options)
        {
            var mlOpsContext = mlOpsBuilderCli.CreateMLOpsContext();

            var experimentId = mlOpsContext.LifeCycle.CreateExperimentAsync(options.ExperimentName).Result;
            Console.WriteLine($"Created experiment {experimentId}");
        }

        public void ListRuns(ListRunsOptions options)
        {
            var mlOpsContext = mlOpsBuilderCli.CreateMLOpsContext();

            var experiment = mlOpsContext.LifeCycle.GetExperiment(options.ExperimentName);
            ConsoleTable.From(experiment.Runs.Select(r => new
            {
                r.RunId,
                r.RunDate
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        public void ListRunArtifacts(ListRunArtifactsOptions options)
        {
            var mlOpsContext = mlOpsBuilderCli.CreateMLOpsContext();

            var runArtifacts = mlOpsContext.LifeCycle.GetRun(options.RunId).RunArtifacts;
            ConsoleTable.From(runArtifacts.Select(r => new
            {
                r.RunArtifactId,
                r.Name
            }))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.MarkDown);
        }

        public void ListMetrics(ListMetricsOptions options)
        {
            var mlOpsContext = mlOpsBuilderCli.CreateMLOpsContext();

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
