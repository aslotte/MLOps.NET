using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    [Verb("ls-metrics", HelpText = "List Metrics for a run")]
    class ListMetricsOptions
    {
        [Option("run-id",Required =true)]
        public Guid RunId { get; set; }
    }
}
