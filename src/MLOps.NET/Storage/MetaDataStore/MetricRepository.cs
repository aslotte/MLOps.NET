using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IMetricRepository"/>
    public sealed class MetricRepository : IMetricRepository
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IMetricRepository"/>
        public MetricRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IMetricRepository"/>
        public async Task LogMetricAsync(Guid runId, string metricName, double metricValue)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var metric = new Metric(runId, metricName, metricValue);
                await db.Metrics.AddAsync(metric);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IMetricRepository"/>
        public List<Metric> GetMetrics(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.Metrics.Where(x => x.RunId == runId).ToList();
            }
        }
    }
}
