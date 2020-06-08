using MLOps.NET.Storage;

namespace MLOps.NET.Azure
{
    /// <summary>
    /// Extension methods to allow the usage of Azure storage
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables the usage of Azure Blobstorage and TableStorage as a storage provider
        /// </summary>
        /// <param name="builder">MLOpsBuilder to add Azure Storage providers to</param>
        /// <param name="connectionString">The connection string for the azure storage account</param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseAzureStorage(this MLOpsBuilder builder, string connectionString)
        {
            builder.UseMetaDataStore(new StorageAccountMetaDataStore(connectionString));
            builder.UseModelRepository(new StorageAccountModelRepository(connectionString));

            return builder;
        }
    }
}
