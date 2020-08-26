using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("config-sql-server", HelpText = "Set configuration values for sql server")]
    public class ConfigSQLServerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("connection-string",Required =true)]
        public string ConnectionString { get; set; }
    }
}
