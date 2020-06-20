using Microsoft.ML.Data;
using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to tracking the evaulation metrics of a model
    /// </summary>
    public sealed class EvaluationCatalog
    {
        private readonly IMetaDataStore metaDataStore;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="metaDataStore"></param>
        public EvaluationCatalog(IMetaDataStore metaDataStore)
        {
            this.metaDataStore = metaDataStore;
        }

        /// <summary>
        /// Logs a given metric for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="metricName"></param>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            await metaDataStore.LogMetricAsync(runId, metricName, metricValue);
        }

        /// <summary>
        /// Logs all evaluation metrics of type double for a machine learning model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runId"></param>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public async Task LogMetricsAsync<T>(Guid runId, T metrics) where T : class
        {
            var metricsType = metrics.GetType();

            var properties = metricsType.GetProperties().Where(x => x.PropertyType == typeof(double));

            foreach (var metric in properties)
            {
                await LogMetricAsync(runId, metric.Name, (double)metric.GetValue(metrics));
            }
        }

        /// <summary>
        /// Saves the confusion matrix as a json serialized string
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="confusionMatrix"></param>
        /// <returns></returns>
        public async Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            var conMatrix = new MLOps.NET.Entities.ConfusionMatrix()
            {
                Counts = confusionMatrix.Counts,
                NumberOfClasses = confusionMatrix.NumberOfClasses,
                PerClassPrecision = confusionMatrix.PerClassPrecision,
                PerClassRecall = confusionMatrix.PerClassRecall
            };
            await metaDataStore.LogConfusionMatrixAsync(runId, conMatrix);
        }

        /// <summary>
        /// Gets the confusion matrix for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public MLOps.NET.Entities.ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            return metaDataStore.GetConfusionMatrix(runId);
        }
    }
}
