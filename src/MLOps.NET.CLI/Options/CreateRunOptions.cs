using CommandLine;
using System;

namespace MLOps.NET.CLI
{
    [Verb("create-run", HelpText = "Create a new run")]
    internal sealed class CreateRunOptions
    {
        [Option("experiment-id", Required = true)]
        public Guid ExperimentId { get; set; }
    }
}
