using CommandLine;

namespace MLOps.NET.CLI
{
    [Verb("config-cosmos", HelpText = "Set configuration values for CosmosDb")]
    internal sealed class ConfigCosmosOptions
    {
        [Option("endpoint", Required = true)]
        public string Endpoint { get; set; }

        [Option("accountkey", Required = true)]
        public string AccountKey { get; set; }
    }
}
