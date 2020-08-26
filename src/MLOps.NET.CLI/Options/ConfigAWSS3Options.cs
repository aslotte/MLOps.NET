using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("config-s3", HelpText = "Set configuration values for s3")]
    public class ConfigAWSS3Options
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("access-key-id",Required =true)]
        public string AwsAccessKeyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Option("access-key", Required = true)]
        public string AwsSecretAccessKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Option("region-name", Required = true)]
        public string RegionName { get; set; }
    }
}
