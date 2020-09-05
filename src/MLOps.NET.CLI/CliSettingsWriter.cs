using MLOps.NET.CLI.Settings;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MLOps.NET.CLI
{
    internal class CliSettingsWriter
    {
        private readonly string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "appsettings.json");

        internal void UpdateSQLServer(ConfigSQLServerOptions options)
        {
            var setting = GetSettings();
            setting.SQLServer.ConnectionString = options.ConnectionString;
            SaveSettings(setting);
        }

        public void SetCosmosConfiguration(ConfigCosmosOptions options)
        {
            var setting = GetSettings();
            setting.CosmosDb.EndPoint = options.Endpoint;
            setting.CosmosDb.AccountKey = options.AccountKey;
            SaveSettings(setting);
        }

        internal void UpdateStorageProvider(SetStorageProviderOptions options)
        {
            var setting = GetSettings();
            setting.DataSource = options.DataSource;
            setting.ModelRepository = options.ModelRepository;
            SaveSettings(setting);
        }

        internal void UpdateS3ModelRepository(ConfigAWSS3Options options)
        {
            var setting = GetSettings();
            setting.S3Config.AwsAccessKeyId = options.AwsAccessKeyId;
            setting.S3Config.AwsSecretAccessKey = options.AwsSecretAccessKey;
            setting.S3Config.RegionName = options.RegionName;
            SaveSettings(setting);
        }

        public CliSettings GetSettings()
        {
            if (!File.Exists(settingsPath))
            {
                File.WriteAllText(settingsPath, string.Empty);
            }
            var json = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<CliSettings>(json) ?? new CliSettings();
        }

        private void SaveSettings(CliSettings settings)
        {
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings));
        }
    }
}
