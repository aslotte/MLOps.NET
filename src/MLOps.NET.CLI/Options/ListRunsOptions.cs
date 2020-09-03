using CommandLine;

namespace MLOps.NET.CLI
{
    [Verb("ls-runs", HelpText = "List runs for a run")]
    internal sealed class ListRunsOptions
    {
        [Option("experiment-name", Required = true)]
        public string ExperimentName { get; set; }
    }
}
