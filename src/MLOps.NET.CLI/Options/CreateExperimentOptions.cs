using CommandLine;

namespace MLOps.NET.CLI
{
    [Verb("create-experiment", HelpText = "Create a new experiment")]
    internal sealed class CreateExperimentOptions
    {
        [Option("name", Required = true)]
        public string ExperimentName { get; set; }
    }
}
