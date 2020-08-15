using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class ExperimentResolver : IEntityResolver<Experiment>
    {
        private readonly IEntityResolver<Run> runBuilder;

        public ExperimentResolver(IEntityResolver<Run> runResolver)
        {
            this.runBuilder = runResolver;
        }

        public Experiment BuildEntity(IMLOpsDbContext db, Experiment entity)
        {
            if (entity.Runs == null)
            {
                db.Entry(entity).Collection(x => x.Runs).Load();
                entity.Runs.ForEach(run => this.runBuilder.BuildEntity(db, run));
            }
            return entity;
        }

        public List<Experiment> BuildEntities(IMLOpsDbContext db, List<Experiment> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
