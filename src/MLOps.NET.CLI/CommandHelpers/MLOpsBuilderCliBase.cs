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
    internal class MLOpsBuilderCliBase
    {
        protected IMLOpsContext mlOpsContext;
        protected readonly CliSettingsWriter settingsHelper;

        public MLOpsBuilderCliBase(CliSettingsWriter settingsHelper)
        {
            this.settingsHelper = settingsHelper;
        }

        protected IMLOpsContext CreateMLOpsContext()
        {
            var settings = settingsHelper.GetSettings();

            var mlOpsBuilder = new NET.MLOpsBuilder();

            UpdateDataSource(settings, mlOpsBuilder);
            UpdateModelRepository(settings, mlOpsBuilder);

            return mlOpsBuilder.Build();
        }

        protected void CreateMLOpsContextIfNotExists()
        {
            if (mlOpsContext == null)
            {
                mlOpsContext = CreateMLOpsContext();
            }
        }

        private void UpdateDataSource(CliSettings settings, NET.MLOpsBuilder mlOpsBuilder)
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

        private void UpdateModelRepository(CliSettings settings, NET.MLOpsBuilder mlOpsBuilder)
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
