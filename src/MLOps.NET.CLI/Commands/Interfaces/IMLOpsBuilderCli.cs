using MLOps.NET.CLI;

namespace MLOps.NET.CLI.Commands.Interfaces
{
    internal interface IMLOpsBuilderCli
    {
        IMLOpsContext CreateMLOpsContext();

        void ConfigureSQLServer(ConfigSQLServerOptions options);

        void ConfigureS3ModelRepository(ConfigAWSS3Options options);

        void ConfigureCosmosDb(ConfigCosmosOptions options);

        void SetStorageProvider(SetStorageProviderOptions options);
    }
}
