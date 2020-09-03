namespace MLOps.NET.CLI.Settings
{
    internal sealed class CliSettings
    {
        public CosmosConfig CosmosDb { get; set; } = new CosmosConfig();

        public SQLiteConfig SQLite { get; set; } = new SQLiteConfig();

        public SQLServerConfig SQLServer { get; set; } = new SQLServerConfig();

        public AWSS3Config S3Config { get; set; } = new AWSS3Config();

        public BlobStorageConfig BlobStorageConfig { get; set; } = new BlobStorageConfig();

        public DataSource DataSource { get; set; } = DataSource.CosmosDb;

        public ModelRepository ModelRepository { get; set; } = ModelRepository.LocalFile;
    }

    internal sealed class BlobStorageConfig
    {
        public string ConnectionString { get; set; }
    }

    internal sealed class AWSS3Config
    {
        public string AwsAccessKeyId { get; set; }

        public string AwsSecretAccessKey { get; set; }

        public string RegionName { get; set; }
    }

    internal sealed class SQLServerConfig
    {
        public string ConnectionString { get; set; }
    }

    internal sealed class SQLiteConfig
    {
        public string Path { get; set; }
    }

    internal sealed class CosmosConfig
    {
        public string EndPoint { get; set; }

        public string AccountKey { get; set; }
    }

    internal enum DataSource
    {
        CosmosDb,
        SQLite,
        SQLServer
    }

    internal enum ModelRepository
    {
        S3,
        LocalFile,
        BlobStorage
    }
}
