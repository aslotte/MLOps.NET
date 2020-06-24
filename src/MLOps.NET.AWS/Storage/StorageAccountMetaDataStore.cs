using MLOps.NET.Entities;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountMetaDataStore : IMetaDataStore
    {
        public Task<Guid> CreateExperimentAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            throw new NotImplementedException();
        }

        public ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            throw new NotImplementedException();
        }

        public IExperiment GetExperiment(string experimentName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IExperiment> GetExperiments()
        {
            throw new NotImplementedException();
        }

        public List<IMetric> GetMetrics(Guid runId)
        {
            throw new NotImplementedException();
        }

        public IRun GetRun(Guid runId)
        {
            throw new NotImplementedException();
        }

        public List<IRun> GetRuns(Guid experimentId)
        {
            throw new NotImplementedException();
        }

        public Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            throw new NotImplementedException();
        }

        public Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            throw new NotImplementedException();
        }

        public Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            throw new NotImplementedException();
        }

        public Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
