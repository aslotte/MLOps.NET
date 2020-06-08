using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLOps.NET.Entities.Entities;
using MLOps.NET.Storage;

namespace MLOps.NET.Tests.Mocks.Storage
{
    public class MockMetaDataStore : IMetaDataStore
    {
        public async Task<Guid> CreateExperimentAsync(string name)
        {
            return Guid.NewGuid();
        }

        public async Task<Guid> CreateRunAsync(Guid experimentId)
        {
            return Guid.NewGuid();
        }

        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
        }

        public IEnumerable<IExperiment> GetExperiments()
        {
            return Enumerable.Empty<IExperiment>();
        }

        public IExperiment GetExperiment(string experimentName)
        {
            return null;
        }

        public List<IRun> GetRuns(Guid experimentId)
        {
            return new List<IRun>();
        }

        public List<IMetric> GetMetrics(Guid runId)
        {
            return new List<IMetric>();
        }
    }
}