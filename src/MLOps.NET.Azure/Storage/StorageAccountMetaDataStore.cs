using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Azure.Entities;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var experiment = new Experiment(name);
            var addedExperiment = await InsertOrMerge(experiment, nameof(Experiment));

            return addedExperiment.Id;
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

        private async Task<IEnumerable<TEntity>> RetrieveEntities<TEntity>(string propertyName, Guid propertyValue, string tableName) where TEntity : ITableEntity, new()
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var items = new List<TEntity>();
            Action<IList<TEntity>> onProgress = null;
            TableContinuationToken token = null;
            CancellationToken ct = default(CancellationToken);
            CloudTable table = tableClient.GetTableReference(tableName);
            TableQuery<TEntity> query = new TableQuery<TEntity>()
                                  .Where(TableQuery.GenerateFilterConditionForGuid(propertyName, QueryComparisons.Equal, propertyValue));
            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync<TEntity>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null) onProgress(items);
            }
            while (token != null && !ct.IsCancellationRequested);
            return items;
        }

        public async Task<IEnumerable<IRun>> GetAllRunsByExperimentIdAsync(Guid experimentId)
        {
            return await RetrieveEntities<Run>("ExperimentId", experimentId, nameof(Run));
        }

        public async Task<IEnumerable<IRun>> GetAllMetricsByRunIdAsync(Guid runId)
        {
            return await RetrieveEntities<Run>("RunId", runId, nameof(Metric));
        }

        public async Task<Dictionary<IRun,IEnumerable<IMetric>>> GetAllRunsAndMetricsByExperimentIdAsync(Guid experimentId)
        {
            var metricTable = await GetTable(nameof(Metric));
            var allRuns = await RetrieveEntities<Run>("ExperimentId", experimentId, nameof(Run));
            var allMetrics = new Dictionary<IRun, IEnumerable<IMetric>>();

            foreach (var run in allRuns)
            {
                var metrics = await RetrieveEntities<Metric>("RunId", run.Id, nameof(Metric));
                allMetrics.Add(run,metrics);
            }
            return allMetrics;
        }
    }
}
