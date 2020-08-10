using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{

    internal sealed class DeploymentBuilder : IEntityBuilder<Deployment>
    {
        private readonly IEntityBuilder<RegisteredModel> registeredModelBuilder;
        private readonly IEntityBuilder<DeploymentTarget> deploymentTargetBuiler;

        public DeploymentBuilder()
        {
            this.registeredModelBuilder = new RegisteredModelBuilder();
            this.deploymentTargetBuiler = new DeploymentTargetBuilder();
        }

        public Deployment BuildEntity(IMLOpsDbContext db, Deployment entity)
        {
            entity.RegisteredModel = db.RegisteredModels.First(x => x.RegisteredModelId == entity.RegisteredModelId);
            this.registeredModelBuilder.BuildEntity(db, entity.RegisteredModel);

            entity.Experiment = db.Experiments.First(x => x.ExperimentId == entity.ExperimentId);

            entity.DeploymentTarget = db.DeploymentTargets.First(x => x.DeploymentTargetId == entity.DeploymentTargetId);
            this.deploymentTargetBuiler.BuildEntity(db, entity.DeploymentTarget);

            return entity;
        }
    }
}
