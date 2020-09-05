using CommandLine;
using MLOps.NET.CLI.Settings;

namespace MLOps.NET.CLI
{
    [Verb("set-storage-provider", HelpText = "Set storage provider")]
    internal sealed class SetStorageProviderOptions
    {
        [Option("datasource", Required = true)]
        public DataSource DataSource { get; set; }

        [Option("model-repository", Required = true)]
        public ModelRepository ModelRepository { get; set; }
    }
}
