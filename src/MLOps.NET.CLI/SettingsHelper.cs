using ConsoleTables;
using Dynamitey.DynamicObjects;
using mlops.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// Helper method to read and update settings
    /// </summary>
    internal class SettingsHelper
    {
        private readonly string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops", "appsettings.json");

        public DataSource GetDataSource()
        {
            var json = File.ReadAllText(settingsPath);
            var setting = JsonConvert.DeserializeObject<Settings>(json);
            return setting.DataSource;
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

        internal void UpdateDataSource(SetDataSourceOptions options)
        {
            var setting = GetSettings();
            setting = setting ?? new Settings();
            setting.DataSource = options.DataSource;
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
