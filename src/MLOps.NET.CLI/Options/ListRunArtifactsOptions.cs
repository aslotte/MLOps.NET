using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.CLI
{
    [Verb("ls-artifacts", HelpText = "List run artifacts by run id")]
    class ListRunArtifactsOptions
    {
        [Option("run-id",Required =true)]
        public Guid RunId { get; set; }
    }
}
