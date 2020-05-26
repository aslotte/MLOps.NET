using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities;
using System;
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

        public async Task<Experiment> CreateExperimentAsync(Experiment experiment)
        {
            return await InsertOrMerge(experiment, nameof(Experiment));
        }

        public async Task<Run> CreateRunAsync(Run run)
        {
            return await InsertOrMerge(run, nameof(Run));
        }

        public async Task LogMetricAsync(Metric metric)
        {
            await InsertOrMerge(metric, nameof(Metric));
        }

        private async Task<CloudTable> GetTable(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }

        private async Task<TEntity> InsertOrMerge<TEntity>(TEntity entity, string tableName) where TEntity : TableEntity
        {
            var table = await GetTable(tableName);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

            return result.Result as TEntity;
        }

        private async Task<TEntity> RetrieveEntity<TEntity>(Guid partitionKey, Guid rowKey, string tableName) where TEntity : TableEntity
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey.ToString(), rowKey.ToString());
            var result = await table.ExecuteAsync(retrieveOperation);

            return result.Result as TEntity;
        }
    }
}
