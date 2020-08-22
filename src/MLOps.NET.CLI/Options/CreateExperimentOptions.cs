using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("create-exp", HelpText = "Create a new experiment")]
    public class CreateExperimentOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Option("name", Required = true)]
        public string ExperimentName { get; set; }
    }
}
