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

            Parser.Default.ParseArguments<SetStorageProviderOptions, ConfigAWSS3Options, ConfigSQLServerOptions,ListRunsOptions, ListRunArtifactsOptions, 
                    ListMetricsOptions,ConfigCosmosOptions, CreateExperimentOptions, CreateRunOptions>(args)
                    .WithParsed<SetStorageProviderOptions>(commandHelper.UpdateStorageProvider)
                    .WithParsed<ConfigAWSS3Options>(commandHelper.UpdateS3ModelRepository)
                    .WithParsed<ConfigSQLServerOptions>(commandHelper.UpdateSQLServer)
                    .WithParsed<ListRunsOptions>(commandHelper.ListRuns)
                    .WithParsed<ListRunArtifactsOptions>(commandHelper.ListRunArtifacts)
                    .WithParsed<ListMetricsOptions>(commandHelper.ListMetrics)
                    .WithParsed<ConfigCosmosOptions>(commandHelper.SetCosmosConfiguration)
                    .WithParsed<CreateExperimentOptions>(commandHelper.CreateExperiment)
                    .WithParsed<CreateRunOptions>(commandHelper.CreateRun);
        }

    }
}
