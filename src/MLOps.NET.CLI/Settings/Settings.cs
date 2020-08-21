using Microsoft.Extensions.DependencyInjection;
using MLOps.NET.Azure.Storage;
using MLOps.NET.CLI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace mlops.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        public CosmosConfig CosmosDb { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SQLiteConfig SQLite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SQLServerConfig SQLServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DataSource DataSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void SetCosmosConfiguration(ConfigCosmosOptions options)
        {
            var json = File.ReadAllText("appsettings.json");
            var setting = JsonConvert.DeserializeObject<Settings>(json);
            setting.CosmosDb.EndPoint = options.Endpoint;
            setting.CosmosDb.AccountKey = options.AccountKey;
            File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(setting));
        }
}

    /// <summary>
    /// 
    /// </summary>
    public class SQLServerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SQLiteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CosmosConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountKey { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// 
        /// </summary>
        CosmosDb,
        /// <summary>
        /// 
        /// </summary>
        SQLite,
        /// <summary>
        /// 
        /// </summary>
        SQLServer
    }
}
