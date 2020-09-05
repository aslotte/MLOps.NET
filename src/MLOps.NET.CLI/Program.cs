using CommandLine;

namespace MLOps.NET.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var cliSettingsWriter = new CliSettingsWriter();

            var mlOpsBuilderCli = new MLOpsBuilderCli(cliSettingsWriter);
            var lifeCycleCatalogCli = new LifeCycleCatalogCli(mlOpsBuilderCli);

            Parser.Default.ParseArguments<SetStorageProviderOptions, ConfigAWSS3Options, ConfigSQLServerOptions, ListRunsOptions, ListRunArtifactsOptions,
                    ListMetricsOptions, ConfigCosmosOptions, CreateExperimentOptions, CreateRunOptions>(args)
                    .WithParsed<SetStorageProviderOptions>(mlOpsBuilderCli.SetStorageProvider)
                    .WithParsed<ConfigAWSS3Options>(mlOpsBuilderCli.ConfigureS3ModelRepository)
                    .WithParsed<ConfigSQLServerOptions>(mlOpsBuilderCli.ConfigureSQLServer)
                    .WithParsed<ListRunsOptions>(lifeCycleCatalogCli.ListRuns)
                    .WithParsed<ListRunArtifactsOptions>(lifeCycleCatalogCli.ListRunArtifacts)
                    .WithParsed<ListMetricsOptions>(lifeCycleCatalogCli.ListMetrics)
                    .WithParsed<ConfigCosmosOptions>(mlOpsBuilderCli.ConfigureCosmosDb)
                    .WithParsed<CreateExperimentOptions>(lifeCycleCatalogCli.CreateExperiment)
                    .WithParsed<CreateRunOptions>(lifeCycleCatalogCli.CreateRun);
        }

    }
}
