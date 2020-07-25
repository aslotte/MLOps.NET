using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IExperimentRepository"/>
    public sealed class ExperimentRepository : IExperimentRepository
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IExperimentRepository"/>
        public ExperimentRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IExperimentRepository"/>
        public async Task<Guid> CreateExperimentAsync(string experimentName)
        {
            using var db = this.contextFactory.CreateDbContext();

            var existingExperiment = db.Experiments.FirstOrDefault(x => x.ExperimentName == experimentName);
            if (existingExperiment == null)
            {
                var experiment = new Experiment(experimentName);
                await db.Experiments.AddAsync(experiment);
                await db.SaveChangesAsync();

                return experiment.ExperimentId;
            }
            return existingExperiment.ExperimentId;
        }

        ///<inheritdoc cref="IExperimentRepository"/>
        public Experiment GetExperiment(string experimentName)
        {
            using var db = this.contextFactory.CreateDbContext();

            return db.Experiments
            .Single(x => x.ExperimentName == experimentName);
        }

        ///<inheritdoc cref="IExperimentRepository"/>
        public IEnumerable<Experiment> GetExperiments()
        {
            using var db = this.contextFactory.CreateDbContext();
            return db.Experiments;
        }
    }
}
