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
            var mLOpsBuilderCliHelper = new MLOpsBuilderCliHelper(new CliSettingsWriter());
            var lifeCycleCatalogCliHelper = new LifeCycleCatalogCliHelper(new CliSettingsWriter());

            Parser.Default.ParseArguments<SetStorageProviderOptions, ConfigAWSS3Options, ConfigSQLServerOptions,ListRunsOptions,ListRunArtifactsOptions, 
                    ListMetricsOptions,ConfigCosmosOptions, CreateExperimentOptions, CreateRunOptions>(args)
                    .WithParsed<SetStorageProviderOptions>(mLOpsBuilderCliHelper.UpdateStorageProvider)
                    .WithParsed<ConfigAWSS3Options>(mLOpsBuilderCliHelper.UpdateS3ModelRepository)
                    .WithParsed<ConfigSQLServerOptions>(mLOpsBuilderCliHelper.UpdateSQLServer)
                    .WithParsed<ListRunsOptions>(lifeCycleCatalogCliHelper.ListRuns)
                    .WithParsed<ListRunArtifactsOptions>(lifeCycleCatalogCliHelper.ListRunArtifacts)
                    .WithParsed<ListMetricsOptions>(lifeCycleCatalogCliHelper.ListMetrics)
                    .WithParsed<ConfigCosmosOptions>(mLOpsBuilderCliHelper.SetCosmosConfiguration)
                    .WithParsed<CreateExperimentOptions>(lifeCycleCatalogCliHelper.CreateExperiment)
                    .WithParsed<CreateRunOptions>(lifeCycleCatalogCliHelper.CreateRun);
        }

    }
}
