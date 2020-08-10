using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class RunBuilder : IEntityBuilder<Run>
    {
        private readonly IEntityBuilder<RunArtifact> runArtifactBuilder;
        private readonly IEntityBuilder<Experiment> experimentBuilder;

        public RunBuilder()
        {
            this.runArtifactBuilder = new RunArtifactBuilder();
            this.experimentBuilder = new ExperimentBuilder();
        }

        public Run BuildEntity(IMLOpsDbContext db, Run entity)
        {
            if (entity.Experiment == null)
            {
                entity.Experiment = db.Experiments.First(x => x.ExperimentId == entity.ExperimentId);
                this.experimentBuilder.BuildEntity(db, entity.Experiment);
            }

            if (entity.HyperParameters == null)
            {
                entity.HyperParameters = db.HyperParameters.Where(x => x.RunId == entity.RunId).ToList();
            }

            if (entity.Metrics == null)
            {
                entity.Metrics = db.Metrics.Where(x => x.RunId == entity.RunId).ToList();
            }

            if (entity.ConfusionMatrix == null)
            {
                entity.ConfusionMatrix = db.ConfusionMatrices.FirstOrDefault(x => x.RunId == entity.RunId);
            }

            if (entity.RunArtifacts == null)
            { 
                entity.RunArtifacts = db.RunArtifacts.Where(x => x.RunId == entity.RunId).ToList();
                entity.RunArtifacts.ForEach(x => this.runArtifactBuilder.BuildEntity(db, x));
            }
            return entity;
        }
    }
}
