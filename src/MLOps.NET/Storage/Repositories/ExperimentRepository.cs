using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityResolvers;
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
        private readonly IEntityResolver<Experiment> experimentResolver;

        ///<inheritdoc cref="IExperimentRepository"/>
        public ExperimentRepository(IDbContextFactory contextFactory, IEntityResolver<Experiment> experimentResolver)
        {
            this.contextFactory = contextFactory;
            this.experimentResolver = experimentResolver;
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

            var experiment = db.Experiments.Single(x => x.ExperimentName == experimentName);

            return this.experimentResolver.BuildEntity(db, experiment);
        }

        ///<inheritdoc cref="IExperimentRepository"/>
        public Experiment GetExperiment(Guid experimentId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var experiment = db.Experiments.Single(x => x.ExperimentId == experimentId);

            return this.experimentResolver.BuildEntity(db, experiment);
        }

        ///<inheritdoc cref="IExperimentRepository"/>
        public IEnumerable<Experiment> GetExperiments()
        {
            using var db = this.contextFactory.CreateDbContext();

            var experiments = db.Experiments.ToList();

            return this.experimentResolver.BuildEntities(db, experiments);
        }
    }
}
