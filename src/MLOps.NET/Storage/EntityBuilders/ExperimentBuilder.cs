using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class ExperimentBuilder : IEntityBuilder<Experiment>
    {
        private readonly IEntityBuilder<Run> runBuilder;

        public ExperimentBuilder()
        {
            this.runBuilder = new RunBuilder();
        }

        public Experiment BuildEntity(IMLOpsDbContext db, Experiment entity)
        {
            if (entity.Runs == null)
            {
                entity.Runs = db.Runs.Where(x => x.ExperimentId == entity.ExperimentId).ToList();
                entity.Runs.ForEach(run => this.runBuilder.BuildEntity(db, run));
            }
            return entity;
        }
    }
}
