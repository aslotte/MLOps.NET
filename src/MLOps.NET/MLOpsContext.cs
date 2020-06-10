using Dynamitey;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MLOps.NET
{
    ///<inheritdoc cref="IMLOpsContext"/>
    public class MLOpsContext : IMLOpsContext
    {
        private readonly IMetaDataStore metaDataStore;
        private readonly IModelRepository modelRepository;

        internal MLOpsContext(IMetaDataStore metaDataStore, IModelRepository modelRepository)
        {
            this.metaDataStore = metaDataStore ?? throw new ArgumentNullException(nameof(metaDataStore));
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            return await metaDataStore.CreateExperimentAsync(name);
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            return await metaDataStore.CreateRunAsync(experimentId);
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateRunAsync(string experimentName)
        {
            var experimentId = await CreateExperimentAsync(experimentName);
            return await CreateRunAsync(experimentId);
        }

        ///<inheritdoc/>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            await metaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        ///<inheritdoc/>
        public async Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class
        {
            var metricsType = metrics.GetType();

            var properties = metricsType.GetProperties().Where(x => x.PropertyType == typeof(double));

            foreach (var metric in properties)
            {
                await LogMetricAsync(runId, metric.Name, (double)metric.GetValue(metrics));
            }
        }

        ///<inheritdoc/>
        public async Task UploadModelAsync(Guid runId, string filePath)
        {
            await modelRepository.UploadModelAsync(runId, filePath);
        }

        ///<inheritdoc/>
        public IRun GetBestRun(Guid experimentId, string metricName)
        {
            var allRuns = metaDataStore.GetRuns(experimentId);
            var bestRunId = allRuns.SelectMany(r => r.Metrics)
                .Where(m => m.MetricName.ToLowerInvariant() == metricName.ToLowerInvariant())
                .OrderByDescending(m => m.Value)
                .First().RunId;

            return allRuns.FirstOrDefault(r => r.Id == bestRunId);
        }

        ///<inheritdoc/>
        public async Task LogHyperParametersAsync<T>(Guid runId, T trainer) where T : class
        {
            var trainerType = trainer.GetType();
            foreach (var trainerField in trainerType.GetRuntimeFields())
            {
                // All trainers have an Options object which is used to set the parameters for the training.
                if (trainerField.Name.Contains("Options"))
                {
                    var options = Dynamic.InvokeGet(trainer, trainerField.Name);
                    foreach (var optionField in trainerField.FieldType.GetFields())
                    {
                        // Tracking only primitive types as of now
                        if (optionField.FieldType.IsPrimitive || optionField.FieldType == typeof(Decimal) || optionField.FieldType == typeof(String))
                        {
                            object value = optionField.GetValue(options);
                            if (value != null)
                                await LogHyperParameterAsync(runId, optionField.Name, optionField.GetValue(options).ToString());
                        }
                    }
                }
            }
        }

        private async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            await metaDataStore.LogHyperParameterAsync(runId, name, value);
        }
    }
}
