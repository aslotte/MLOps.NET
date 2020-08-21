using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("config-cosmos", HelpText = "Set configuration values for cosmos db")]
    public class ConfigCosmosOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("endpoint",Required =true)]
        public string Endpoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Option("accountkey", Required = true)]
        public string AccountKey { get; set; }
    }
}
