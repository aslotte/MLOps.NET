using mlops.Settings;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// Helper method to read and update settings
    /// </summary>
    internal class SettingsHelper
    {
        private readonly string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "appsettings.json");

        internal void UpdateSQLServer(ConfigSQLServerOptions options)
        {
            var setting = GetSettings();
            setting.SQLServer.ConnectionString = options.ConnectionString;
            SaveSettings(setting);
        }

        /// <summary>
        /// Set Cosmos Configuration
        /// </summary>
        /// <param name="options"></param>
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
            setting ??= new Settings();
            setting.DataSource = options.DataSource;
            setting.ModelRepository = options.ModelRepository;
            SaveSettings(setting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal void UpdateS3ModelRepository(ConfigAWSS3Options options)
        {
            var setting = GetSettings();
            setting ??= new Settings();
            setting.S3Config = new AWSS3Config
            {
                AwsAccessKeyId = options.AwsAccessKeyId,
                AwsSecretAccessKey = options.AwsSecretAccessKey,
                RegionName = options.RegionName
            };
            SaveSettings(setting);
        }

        /// <summary>
        /// Get Settings
        /// </summary>
        /// <returns></returns>
        public Settings GetSettings()
        {
            if (!File.Exists(settingsPath))
            {
                File.WriteAllText(settingsPath, "");
            }
            var json = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        /// <summary>
        /// Get Settings
        /// </summary>
        /// <returns></returns>
        private void SaveSettings(Settings settings)
        {
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings));
        }
    }
}
