using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("create-run", HelpText = "Create a new run")]
    public class CreateRunOptions
    {
        /// <summary>
        /// Experiment Id
        /// </summary>
        [Option("exp-id", Required = true)]
        public Guid ExperimentId { get; set; }
    }
}
