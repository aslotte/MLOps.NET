using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents.SystemFunctions;
using MLOps.NET.Azure.Entities;
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

        public async Task<Guid> CreateExperimentAsync(string name)
        {
            // Check if experiment exists
            var existingExperiment = await RetrieveEntityAsync<Experiment>(name, name, nameof(Experiment));

            // Add if it doesn't exist
            if(existingExperiment == null)
            {
                var experiment = new Experiment(name);
                var addedExperiment = await InsertOrMerge(experiment, nameof(Experiment));
                return addedExperiment.Id;
            }
            
            // Return existing id if exists
            return existingExperiment.Id;
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            var run = new Run(experimentId);
            var addedRun = await InsertOrMerge(run, nameof(Run));

            return addedRun.Id;
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            var metric = new Metric(runId, metricName, metricValue);

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

        private async Task<TEntity> RetrieveEntityAsync<TEntity>(string partitionKey, string rowKey, string tableName) where TEntity : TableEntity
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);

            return result.Result as TEntity;
        }
    }
}
