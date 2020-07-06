using Microsoft.Azure.Cosmos.Table;
using Microsoft.ML;
using MLOps.NET.Azure.Entities;
using MLOps.NET.Entities;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Entities.Interfaces;
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
            var existingExperiment = RetrieveEntity<Experiment>(x => x.ExperimentName == name, nameof(Experiment));

            if (existingExperiment == null)
            {
                var experiment = new Experiment(name);
                var addedExperiment = await InsertOrMergeAsync(experiment, nameof(Experiment));
                return addedExperiment.Id;
            }

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

        public IRun GetRun(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var runTable = tableClient.GetTableReference(nameof(Run));

            return runTable.CreateQuery<Run>().FirstOrDefault(x => x.Id == runId);
        }

        public List<IMetric> GetMetrics(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var metricTable = tableClient.GetTableReference(nameof(Metric));

            var metrics = metricTable.CreateQuery<Metric>()
                .Where(x => x.RunId == runId)
                .ToList<IMetric>();

            return metrics;
        }

        public ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var confusionMatrixTable = tableClient.GetTableReference(nameof(ConfusionMatrix));

            var confusionMatrix = confusionMatrixTable.CreateQuery<ConfusionMatrixEntity>()
                .FirstOrDefault(x => x.RunId == runId);
                
            if (confusionMatrix  == null) return null;         

            return JsonConvert.DeserializeObject<ConfusionMatrix>(confusionMatrix.SerializedMatrix);
        }

        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            var hyperParameter = new HyperParameter(runId, name, value);

            await InsertOrMergeAsync(hyperParameter, nameof(HyperParameter));
        }

        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            var existingRun = GetRun(runId);
            if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

            existingRun.TrainingTime = timeSpan;

            await InsertOrMergeAsync(existingRun as Run, nameof(Run));
        }

        public async Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            var conMatrix = new ConfusionMatrixEntity(runId)
            {
                SerializedMatrix = JsonConvert.SerializeObject(confusionMatrix)
            };
            await InsertOrMergeAsync(conMatrix, nameof(ConfusionMatrix));
        }

        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            var data = new Data(runId);
            await InsertOrMergeAsync(data, nameof(Data));

            var dataSchema = new DataSchema(data.Id)
            {
                ColumnCount = dataView.Schema.Count()
            };
            await InsertOrMergeAsync(dataSchema, nameof(DataSchema));

            foreach (var column in dataView.Schema)
            {
                var dataColumn = new DataColumn(dataSchema.Id)
                {
                    Name = column.Name,
                    Type = column.Type.ToString()
                };
                await InsertOrMergeAsync(dataColumn, nameof(DataColumn));
            }
        }

        public IData GetData(Guid runId)
        {
            var data = RetrieveEntity<Data>(x => x.RunId == runId, nameof(Data));
            if (data == null) return null;

            data.DataSchema = RetrieveEntity<DataSchema>(x => x.DataId == data.Id, nameof(DataSchema));

            var dataColumns = RetrieveEntities<DataColumn>(x => x.DataSchemaId == data.DataSchema.Id, nameof(DataSchema))
                .AsEnumerable<IDataColumn>()
                .ToList();

            data.DataSchema.DataColumns = dataColumns;

            return data;
        }

        private TEntity RetrieveEntity<TEntity>(Func<TEntity, bool> predicate, string tableName) where TEntity : TableEntity, new()
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);

            return table.CreateQuery<TEntity>().FirstOrDefault(predicate);
        }

        private IEnumerable<TEntity> RetrieveEntities<TEntity>(Func<TEntity, bool> predicate, string tableName) where TEntity : TableEntity, new()
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);

            return table.CreateQuery<TEntity>().Where(predicate);
        }
    }
}
