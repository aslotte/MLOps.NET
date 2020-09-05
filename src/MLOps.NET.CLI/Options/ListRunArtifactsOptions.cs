using CommandLine;
using System;

namespace MLOps.NET.CLI
{
    [Verb("ls-artifacts", HelpText = "List run artifacts by run id")]
    internal sealed class ListRunArtifactsOptions
    {
        [Option("run-id", Required = true)]
        public Guid RunId { get; set; }
    }
}
