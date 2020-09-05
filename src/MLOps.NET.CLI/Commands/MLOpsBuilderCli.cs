using MLOps.NET.AWS;
using MLOps.NET.Azure;
using MLOps.NET.CLI.Commands.Interfaces;
using MLOps.NET.CLI.Settings;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using MLOps.NET.SQLServer;
using System;

namespace MLOps.NET.CLI
{
    internal sealed class MLOpsBuilderCli : IMLOpsBuilderCli
    {
        private readonly CliSettingsWriter settingsHelper;

        public MLOpsBuilderCli(CliSettingsWriter settingsHelper)
        {
            this.settingsHelper = settingsHelper;
        }

        public IMLOpsContext CreateMLOpsContext()
        {
            var settings = settingsHelper.GetSettings();

            var mlOpsBuilder = new MLOpsBuilder();

            UpdateDataSource(settings, mlOpsBuilder);
            UpdateModelRepository(settings, mlOpsBuilder);

            return mlOpsBuilder.Build();
        }

        public void ConfigureSQLServer(ConfigSQLServerOptions options)
        {
            settingsHelper.UpdateSQLServer(options);
            Console.WriteLine($"SQL Server has been configured");
        }

        public void ConfigureS3ModelRepository(ConfigAWSS3Options options)
        {
            settingsHelper.UpdateS3ModelRepository(options);
            Console.WriteLine($"AWS S3 has been configured");
        }

        public void SetStorageProvider(SetStorageProviderOptions options)
        {
            settingsHelper.UpdateStorageProvider(options);
            Console.WriteLine($"The Data Source has been updated to {options.DataSource}");
            Console.WriteLine($"The Model repository has been updated to {options.ModelRepository}");
        }

        public void ConfigureCosmosDb(ConfigCosmosOptions options)
        {
            settingsHelper.SetCosmosConfiguration(options);
            CreateMLOpsContext();
            Console.WriteLine($"CosmosDB has been configured");
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
    }
}
