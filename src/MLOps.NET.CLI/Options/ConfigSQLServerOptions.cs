using CommandLine;

namespace MLOps.NET.CLI
{
    [Verb("config-sql-server", HelpText = "Set configuration values for SQLServer")]
    internal sealed class ConfigSQLServerOptions
    {
        [Option("connection-string", Required = true)]
        public string ConnectionString { get; set; }
    }
}
