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
    /// Class representation of appsettings.json
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// config for cosmosdb
        /// </summary>
        public CosmosConfig CosmosDb { get; set; } = new CosmosConfig();

        /// <summary>
        /// config for sqlite
        /// </summary>
        public SQLiteConfig SQLite { get; set; } = new SQLiteConfig();

        /// <summary>
        /// 
        /// </summary>
        public SQLServerConfig SQLServer { get; set; } = new SQLServerConfig();

        /// <summary>
        /// config for sql server
        /// </summary>
        public DataSource DataSource { get; set; } = DataSource.CosmosDb;

       
    }

    /// <summary>
    /// SQL Server Config
    /// </summary>
    public class SQLServerConfig
    {
        /// <summary>
        /// Connection String
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// SQLite Config
    /// </summary>
    public class SQLiteConfig
    {
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// Cosmos Config
    /// </summary>
    public class CosmosConfig
    {
        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// AccountKey
        /// </summary>
        public string AccountKey { get; set; }
    }

    /// <summary>
    /// DataSource
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// CosmosDb
        /// </summary>
        CosmosDb,
        /// <summary>
        /// SQLite
        /// </summary>
        SQLite,
        /// <summary>
        /// SQLServer
        /// </summary>
        SQLServer
    }
}
