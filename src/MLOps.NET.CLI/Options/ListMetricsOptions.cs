using CommandLine;
using System;

namespace MLOps.NET.CLI
{
    [Verb("ls-metrics", HelpText = "List Metrics for a run")]
    internal sealed class ListMetricsOptions
    {
        [Option("run-id", Required = true)]
        public Guid RunId { get; set; }
    }
}
