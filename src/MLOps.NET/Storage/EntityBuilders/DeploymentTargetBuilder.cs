using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class DeploymentTargetBuilder : IEntityBuilder<DeploymentTarget>
    {
        private readonly IEntityBuilder<Deployment> deploymentBuilder;

        public DeploymentTargetBuilder()
        {
            this.deploymentBuilder = new DeploymentBuilder();
        }

        public DeploymentTarget BuildEntity(IMLOpsDbContext db, DeploymentTarget entity)
        {
            if (entity.Deployments == null)
            {
                if (entity.Deployments == null)
                {
                    entity.Deployments = db.Deployments.Where(x => x.DeploymentTargetId == entity.DeploymentTargetId).ToList();

                    entity.Deployments.ForEach(x =>
                    {
                        this.deploymentBuilder.BuildEntity(db, x);
                    });
                }
            }
            return entity;
        }
    }
}
