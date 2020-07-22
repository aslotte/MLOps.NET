using Microsoft.ML.Data;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to tracking the evaulation metrics of a model
    /// </summary>
    public sealed class EvaluationCatalog
    {
        private readonly IMetricRepository metricRepository;
        private readonly IConfusionMatrixRepository confusionMatrixRepository;
        private readonly IModelLabelRepository modelLabelRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="metricRepository"></param>
        /// <param name="confusionMatrixRepository"></param>
        /// <param name="modelLabelRepository"></param>
        public EvaluationCatalog(IMetricRepository metricRepository, IConfusionMatrixRepository confusionMatrixRepository, IModelLabelRepository modelLabelRepository)
        {
            this.metricRepository = metricRepository;
            this.confusionMatrixRepository = confusionMatrixRepository;
            this.modelLabelRepository = modelLabelRepository;
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
            await metricRepository.LogMetricAsync(runId, metricName, metricValue);
        }

        /// <summary>
        /// Logs a given label for a run
        /// </summary>
        /// <param name="runArtifactId"></param>
        /// <param name="labelName"></param>
        /// <param name="labelValue"></param>
        /// <returns></returns>
        public async Task LogModelLabelAsync(Guid runArtifactId, string labelName, string labelValue)
        {
            await modelLabelRepository.LogModelLabelAsync(runArtifactId, labelName, labelValue);
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
                var value = (double)metric.GetValue(metrics);

                if (double.IsNaN(value)) continue;

                await LogMetricAsync(runId, metric.Name, value);
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
            var conMatrix = new Entities.ConfusionMatrix()
            {
                Counts = confusionMatrix.Counts,
                NumberOfClasses = confusionMatrix.NumberOfClasses,
                PerClassPrecision = confusionMatrix.PerClassPrecision,
                PerClassRecall = confusionMatrix.PerClassRecall
            };
            await confusionMatrixRepository.LogConfusionMatrixAsync(runId, conMatrix);
        }

        /// <summary>
        /// Gets the confusion matrix for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public Entities.ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            return confusionMatrixRepository.GetConfusionMatrix(runId);
        }

        /// <summary>
        /// Get the metrics for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public List<Metric> GetMetrics(Guid runId)
        {
            return metricRepository.GetMetrics(runId);
        }

        /// <summary>
        /// Get the Label for a run
        /// </summary>
        /// <param name="runArtifactId"></param>
        /// <returns></returns>
        public List<ModelLabel> GetModelLabels(Guid runArtifactId)
        {
            return modelLabelRepository.GetModelLabels(runArtifactId);
        }
    }
}
