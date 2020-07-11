using Microsoft.ML;
using MLOps.NET.Entities;
using MLOps.NET.Entities.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IMetaDataStore"/>
    public sealed class MetaDataStore : IMetaDataStore
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IMetaDataStore"/>
        public MetaDataStore(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var existingExperiment = db.Experiments.FirstOrDefault(x => x.ExperimentName == name);
                if (existingExperiment == null)
                {
                    var experiment = new Experiment(name);
                    await db.Experiments.AddAsync(experiment);
                    await db.SaveChangesAsync();

                    return experiment.Id;
                }
                return existingExperiment.Id;
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var run = new Run(experimentId)
                {
                    GitCommitHash = gitCommitHash
                };

                await db.Runs.AddAsync(run);
                await db.SaveChangesAsync();

                return run.Id;
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public Experiment GetExperiment(string experimentName)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Experiments.Single(x => x.ExperimentName == experimentName);
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public IEnumerable<Experiment> GetExperiments()
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Experiments;
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public List<Metric> GetMetrics(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Metrics.Where(x => x.RunId == runId).ToList<Metric>();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>

        public Run GetRun(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Runs.FirstOrDefault(x => x.Id == runId);
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public Run GetRun(string commitHash)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Runs.FirstOrDefault(x => x.GitCommitHash == commitHash);
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public List<Run> GetRuns(Guid experimentId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Runs.Where(x => x.ExperimentId == experimentId).ToList<Run>();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var hyperParameter = new HyperParameter(runId, name, value);
                await db.HyperParameters.AddAsync(hyperParameter);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var metric = new Metric(runId, metricName, metricValue);
                await db.Metrics.AddAsync(metric);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var existingRun = db.Runs.FirstOrDefault(x => x.Id == runId);
                if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

                existingRun.TrainingTime = timeSpan;

                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var conMatrix = new ConfusionMatrixEntity(runId)
                {
                    SerializedMatrix = JsonConvert.SerializeObject(confusionMatrix)
                };

                await db.ConfusionMatrices.AddAsync(conMatrix);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var confusionMatrixEntity = db.ConfusionMatrices.SingleOrDefault(x => x.RunId == runId);

                if (confusionMatrixEntity == null) return null;

                return JsonConvert.DeserializeObject<ConfusionMatrix>(confusionMatrixEntity.SerializedMatrix);
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var data = new Data(runId);

                var dataSchema = new DataSchema(data.Id)
                {
                    ColumnCount = dataView.Schema.Count()
                };

                db.Data.Add(data);
                db.DataSchemas.Add(dataSchema);

                foreach (var column in dataView.Schema)
                {
                    var dataColumn = new DataColumn(dataSchema.Id)
                    {
                        Name = column.Name,
                        Type = column.Type.ToString()
                    };

                    db.DataColumns.Add(dataColumn);
                }

                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetaDataStore"/>
        public Data GetData(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var data = db.Data.FirstOrDefault(x => x.RunId == runId);
                if (data == null) return null;

                data.DataSchema = db.DataSchemas.FirstOrDefault(x => x.DataId == data.Id);
                data.DataSchema.DataColumns = db.DataColumns
                    .Where(x => x.DataSchemaId == data.DataSchema.Id)
                    .AsEnumerable<DataColumn>()
                    .ToList();

                return data;
            }
        }
    }
}
