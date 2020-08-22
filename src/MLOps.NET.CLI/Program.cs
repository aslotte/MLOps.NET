using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MLOps.NET.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var commandHelper = new CommandHelper(new SettingsHelper());

            Parser.Default.ParseArguments<SetDataSourceOptions,ListRunsOptions, ListRunArtifactsOptions, ListMetricsOptions,ConfigCosmosOptions, CreateExperimentOptions>(args)
                    .WithParsed<SetDataSourceOptions>(commandHelper.UpdateDataSource)
                    .WithParsed<ListRunsOptions>(commandHelper.ListRuns)
                    .WithParsed<ListRunArtifactsOptions>(commandHelper.ListRunArtifacts)
                    .WithParsed<ListMetricsOptions>(commandHelper.ListMetrics)
                    .WithParsed<ConfigCosmosOptions>(commandHelper.SetCosmosConfiguration)
                    .WithParsed<CreateExperimentOptions>(commandHelper.CreateExperiment);
        }

    }
}
