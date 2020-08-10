using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class RegisteredModelBuilder : IEntityBuilder<RegisteredModel>
    {
        private readonly IEntityBuilder<Run> runBuilder;
        private readonly IEntityBuilder<Experiment> experimentBuilder;

        public RegisteredModelBuilder()
        {
            this.runBuilder = new RunBuilder();
            this.experimentBuilder = new ExperimentBuilder();
        }

        public RegisteredModel BuildEntity(IMLOpsDbContext db, RegisteredModel entity)
        {
            entity.RunArtifact = db.RunArtifacts.First(x => x.RunArtifactId == entity.RunArtifactId);
            entity.Run = db.Runs.First(x => x.RunId == entity.RunId);
            entity.Experiment = db.Experiments.First(x => x.ExperimentId == entity.ExperimentId);

            experimentBuilder.BuildEntity(db, entity.Experiment);
            runBuilder.BuildEntity(db, entity.Run);

            return entity;
        }
    }
}
