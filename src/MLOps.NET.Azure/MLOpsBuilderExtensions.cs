using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage;

namespace MLOps.NET.Azure
{
    /// <summary>
    /// Extension methods to allow the usage of Azure storage
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables the usage of CosmosDb as a storage provider for model meta data
        /// </summary>
        /// <param name="builder">MLOpsBuilder to add Azure Storage providers to</param>
        /// <param name="accountEndpoint"></param>
        /// <param name="accountKey"></param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseCosmosDb(this MLOpsBuilder builder, string accountEndpoint, string accountKey)
        {
            var options = new DbContextOptionsBuilder()
                .UseCosmos(accountEndpoint, accountKey, databaseName: "MLOpsNET")
                .Options;

            builder.UseMetaDataStore(new MetaDataStore(new DbContextFactory(options)));

            return builder;
        }

        /// <summary>
        /// Enables the usage of Azure Blobstorage as a storage provider for the models
        /// </summary>
        /// <param name="builder">MLOpsBuilder to add Azure Storage providers to</param>
        /// <param name="connectionString">The connection string for the azure storage account</param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseAzureBlobModelRepository(this MLOpsBuilder builder, string connectionString)
        {
            builder.UseModelRepository(new StorageAccountModelRepository(connectionString));

            return builder;
        }
    }
}
