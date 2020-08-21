using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    [Verb("ls-runs", HelpText = "List runs")]
    class ListRunsOptions
    {
        [Option("exp-name",Required =true)]
        public string ExperimentName { get; set; }
    }
}
