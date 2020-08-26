using CommandLine;
using mlops.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("set-storage-provider", HelpText = "Set storage provider")]
    public class SetStorageProviderOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("datasource",Required =true)]
        public DataSource DataSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Option("model-repository", Required =true)]
        public ModelRepository ModelRepository { get; set; }
    }
}
