using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Azure.Entities;
using MLOps.NET.Entities.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (existingExperiment == null)
            {
                var experiment = new Experiment(name);
                var addedExperiment = await InsertOrMergeAsync(experiment, nameof(Experiment));
                return addedExperiment.Id;
            }

            // Return existing id if exists
            return existingExperiment.Id;
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            var run = new Run(experimentId)
            {
                GitCommitHash = gitCommitHash
            };

            var addedRun = await InsertOrMergeAsync(run, nameof(Run));

            return addedRun.Id;
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            var metric = new Metric(runId, metricName, metricValue);

            await InsertOrMergeAsync(metric, nameof(Metric));
        }

        private async Task<CloudTable> GetTableAsync(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }

        private async Task<TEntity> InsertOrMergeAsync<TEntity>(TEntity entity, string tableName) where TEntity : TableEntity
        {
            var table = await GetTableAsync(tableName);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

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

        ///<inheritdoc/>
        public IEnumerable<IExperiment> GetExperiments()
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var experimentTable = tableClient.GetTableReference(nameof(Experiment));
            var experiments = experimentTable.CreateQuery<Experiment>().Execute();

            foreach (var experiment in experiments)
            {
                experiment.Runs = GetRuns(experiment.Id);
            }
            return experiments;
        }

        ///<inheritdoc/>
        public IExperiment GetExperiment(string experimentName)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var experimentTable = tableClient.GetTableReference(nameof(Experiment));

            var experiment = experimentTable.CreateQuery<Experiment>()
                .FirstOrDefault(x => x.ExperimentName == experimentName);

            if (experiment == null) throw new InvalidOperationException($"The experiment named {experimentName} does not exist");

            experiment.Runs = GetRuns(experiment.Id);

            return experiment;
        }

        ///<inheritdoc/>
        public List<IRun> GetRuns(Guid experimentId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var runTable = tableClient.GetTableReference(nameof(Run));

            var runs = runTable.CreateQuery<Run>()
                .Where(x => x.ExperimentId == experimentId)
                .ToList<IRun>();

            foreach (var run in runs)
            {
                run.Metrics = GetMetrics(run.Id);
            }
            return runs;
        }

        ///<inheritdoc/>
        public IRun GetRun(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var runTable = tableClient.GetTableReference(nameof(Run));

            var run = runTable.CreateQuery<Run>().FirstOrDefault(x => x.Id == runId);
            run.Metrics = GetMetrics(run.Id);

            return run;
        }

        ///<inheritdoc/>
        public List<IMetric> GetMetrics(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var metricTable = tableClient.GetTableReference(nameof(Metric));

            var metrics = metricTable.CreateQuery<Metric>()
                .Where(x => x.RunId == runId)
                .ToList<IMetric>();

            return metrics;
        }

        ///<inheritdoc/>
        public IConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var confusionMatrixTable = tableClient.GetTableReference(nameof(ConfusionMatrix));

            var confusionMatrix = confusionMatrixTable.CreateQuery<ConfusionMatrix>()
                .FirstOrDefault(x => x.RunId == runId);

            return confusionMatrix;
        }

        ///<inheritdoc/>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            var hyperParameter = new HyperParameter(runId, name, value);

            await InsertOrMergeAsync(hyperParameter, nameof(HyperParameter));
        }

        ///<inheritdoc/>
        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            var existingRun = GetRun(runId);
            if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

            existingRun.TrainingTime = timeSpan;

            await InsertOrMergeAsync(existingRun as Run, nameof(Run));
        }

        ///<inheritdoc/>
        public async Task LogConfusionMatrixAsync(Guid runId, Microsoft.ML.Data.ConfusionMatrix confusionMatrix)
        {
            var conMatrix = new ConfusionMatrix(runId, confusionMatrix.NumberOfClasses,
                                                confusionMatrix.PerClassPrecision.ToList(),
                                                confusionMatrix.PerClassRecall.ToList(),
                                                confusionMatrix.Counts);
            conMatrix.SerializedDetails = JsonConvert.SerializeObject(conMatrix);
            await InsertOrMergeAsync(conMatrix, nameof(ConfusionMatrix));
        }
    }
}
