using MLOps.NET.Azure;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using MLOps.NET.SQLServer;

namespace MLOps.NET.Benchmarks
{
    internal class BenchmarkSetup
    {
        internal static IMLOpsContext InitializeSQLite()
        {
            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLite()
                .Build();
        }

        internal static IMLOpsContext InitializeSQLServer()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLServer(configuration[ConfigurationKeysSQLServer.ConnectionString])
                .Build();
        }

        internal static IMLOpsContext InitializeAzure()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseCosmosDb(configuration[ConfigurationKeysAzure.CosmosEndPoint],
                configuration[ConfigurationKeysAzure.CosmosAccountKey])
                .Build();
        }
    }
}


