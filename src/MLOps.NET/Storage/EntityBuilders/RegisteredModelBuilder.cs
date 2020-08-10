using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class RegisteredModelBuilder : IEntityBuilder<RegisteredModel>
    {
        private readonly IEntityBuilder<Run> runBuilder;
        private readonly IEntityBuilder<Experiment> experimentBuilder;
        private readonly IEntityBuilder<RunArtifact> runArtifactBuilder;

        public RegisteredModelBuilder()
        {
            this.runBuilder = new RunBuilder();
            this.experimentBuilder = new ExperimentBuilder();
            this.runArtifactBuilder = new RunArtifactBuilder();
        }

        public RegisteredModel BuildEntity(IMLOpsDbContext db, RegisteredModel entity)
        {
            if (entity.RunArtifact == null)
            {
                entity.RunArtifact = db.RunArtifacts.First(x => x.RunArtifactId == entity.RunArtifactId);
                this.runArtifactBuilder.BuildEntity(db, entity.RunArtifact);
            }

            if (entity.Run == null)
            {
                entity.Run = db.Runs.First(x => x.RunId == entity.RunId);
                runBuilder.BuildEntity(db, entity.Run);
            }

            if (entity.Experiment == null)
            {
                entity.Experiment = db.Experiments.First(x => x.ExperimentId == entity.ExperimentId);
                experimentBuilder.BuildEntity(db, entity.Experiment);
            }
            return entity;
        }
    }
}
