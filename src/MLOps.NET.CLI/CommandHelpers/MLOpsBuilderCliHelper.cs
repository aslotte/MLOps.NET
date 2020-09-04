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
    internal class MLOpsBuilderCliHelper : MLOpsBuilderCliBase
    {
        public MLOpsBuilderCliHelper(CliSettingsWriter settingsHelper) : base(settingsHelper)
        {
            
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

        internal void SetCosmosConfiguration(ConfigCosmosOptions options)
        {
            settingsHelper.SetCosmosConfiguration(options);
            CreateMLOpsContext();
            Console.WriteLine($"CosmosDB has been configured");
        }
    }
}
