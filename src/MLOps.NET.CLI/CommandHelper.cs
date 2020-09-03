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
    internal class CommandHelper
    {
        private IMLOpsContext mlOpsContext;
        private readonly CliSettingsWriter settingsHelper;

        public CommandHelper(CliSettingsWriter settingsHelper)
        {
            this.settingsHelper = settingsHelper;
        }

        internal void UpdateSQLServer(ConfigSQLServerOptions options)
        {
            settingsHelper.UpdateSQLServer(options);
            Console.WriteLine($"SQL Server has been configured");
        }

        internal void UpdateS3ModelRepository(ConfigAWSS3Options options)
        {
            settingsHelper.UpdateS3ModelRepository(options);
            Console.WriteLine($"AWS S3 has been configured");
        }

        internal void UpdateStorageProvider(SetStorageProviderOptions options)
        {
            settingsHelper.UpdateStorageProvider(options);
            Console.WriteLine($"The Data Source has been updated to {options.DataSource}");
            Console.WriteLine($"The Model repository has been updated to {options.ModelRepository}");
        }

        internal void CreateRun(CreateRunOptions options)
        {
            CreateMLOpsContextIfNotExists();

            var run = mlOpsContext.LifeCycle.CreateRunAsync(options.ExperimentId).Result;
            Console.WriteLine($"Created Run {run.RunId}");
        }

        private IMLOpsContext CreateMLOpsContext()
        {
            var settings = settingsHelper.GetSettings();

            var mlOpsBuilder = new MLOpsBuilder();

            UpdateDataSource(settings, mlOpsBuilder);
            UpdateModelRepository(settings, mlOpsBuilder);

            return mlOpsBuilder.Build();
        }

        private void UpdateDataSource(CliSettings settings, MLOpsBuilder mlOpsBuilder)
        {
            switch (settings.DataSource)
            {
                case DataSource.CosmosDb:
                    mlOpsBuilder.UseCosmosDb(settings.CosmosDb.EndPoint, settings.CosmosDb.AccountKey);
                    break;
                case DataSource.SQLServer:
                    mlOpsBuilder.UseSQLServer(settings.SQLServer.ConnectionString);
                    break;
                case DataSource.SQLite:
                    mlOpsBuilder.UseSQLite();
                    break;
                default:
                    throw new InvalidOperationException($"The selected data source {settings.DataSource} is not supported");
            }
        }

        private void UpdateModelRepository(CliSettings settings, MLOpsBuilder mlOpsBuilder)
        {
            switch (settings.ModelRepository)
            {
                case ModelRepository.LocalFile:
                    mlOpsBuilder.UseLocalFileModelRepository();
                    break;
                case ModelRepository.S3:
                    mlOpsBuilder.UseAWSS3ModelRepository(settings.S3Config.AwsAccessKeyId, settings.S3Config.AwsSecretAccessKey, settings.S3Config.RegionName);
                    break;
                case ModelRepository.BlobStorage:
                    mlOpsBuilder.UseAzureBlobModelRepository(settings.BlobStorageConfig.ConnectionString);
                    break;
                default:
                    throw new InvalidOperationException($"The selected model repository {settings.ModelRepository} is not supported");
            }
        }

        internal void CreateExperiment(CreateExperimentOptions options)
        {
            CreateMLOpsContextIfNotExists();

            var experimentId = mlOpsContext.LifeCycle.CreateExperimentAsync(options.ExperimentName).Result;
            Console.WriteLine($"Created experiment {experimentId}");
        }

        private void CreateMLOpsContextIfNotExists()
        {
            if (mlOpsContext == null)
            {
                mlOpsContext = CreateMLOpsContext();
            }             
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

        internal void SetCosmosConfiguration(ConfigCosmosOptions options)
        {
            settingsHelper.SetCosmosConfiguration(options);
            CreateMLOpsContext();
            Console.WriteLine($"CosmosDB has been configured");
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
