using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountMetaDataStore : IMetaDataStore
    {
        private readonly CloudStorageAccount storageAccount;

        public StorageAccountMetaDataStore(string connectionString)
        {
            this.storageAccount = EnsureStorageIsCreated.CreateStorageAccountFromConnectionString(connectionString);
        }

        public async Task<Experiment> CreateExperiementAsync(Experiment experiment)
        {
            return await InsertOrMerge(experiment);
        }

        public async Task<Run> CreateRunAsync(Run run)
        {
            return await InsertOrMerge(run);
        }

        private async Task<CloudTable> GetTable(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }

        private async Task<TEntity> InsertOrMerge<TEntity>(TEntity entity) where TEntity : TableEntity
        {
            var table = await GetTable(nameof(TEntity));
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

            return result.Result as TEntity;
        }
    }
}
